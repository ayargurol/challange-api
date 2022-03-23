using AutoMapper;
using Challange.Core.Common;
using Challange.Core.Repositories;
using Challenge.Application.Abstract;
using Challenge.Application.Dto.User.Request;
using Challenge.Application.Dto.User.Response;
using Challenge.Application.Mapping;
using Challenge.Domain;
using Challenge.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Challenge.Application.Concrete;

public class UserManager : IUserManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenManager _tokenManager;
    private readonly AppSettings _appSettings;
    private readonly IMapper _mapper;

    public UserManager(IUnitOfWork unitOfWork, ITokenManager tokenManager, IOptions<AppSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _tokenManager = tokenManager;
        _appSettings = appSettings.Value;
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>()).CreateMapper();
    }

    public async Task<User> GetById(Guid id)
    {
        return await _unitOfWork.Repository<IUserRepository>().GetAsync(a => a.Id == id);
    }

    public async Task Register(RegisterRequest request)
    {
        var user = _mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        _unitOfWork.Repository<IUserRepository>().Add(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Response<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _unitOfWork.Repository<IUserRepository>().GetAsync(x => x.Email == request.Email, x => x.RefreshTokens);

        // validate
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Username or password is incorrect");

        // authentication successful so generate jwt and refresh tokens
        var jwtToken = _tokenManager.GenerateJwtToken(user);
        var refreshToken = _tokenManager.GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);

        // remove old refresh tokens from user
        removeOldRefreshTokens(user);

        // save changes to db
        user = _unitOfWork.Repository<IUserRepository>().Update(user);
        await _unitOfWork.SaveChangesAsync();
        return Response<LoginResponse>.CreateResponse(new LoginResponse
        {
            Id = user.Id,
            FirstName = user.Firstname,
            LastName = user.Lastname,
            Email = user.Email,
            JwtToken = jwtToken,
            Role = user.Role,
            RefreshToken = refreshToken.Token
        }, StatusCodes.Status200OK);
    }

    public async Task<Response<LoginResponse>> RefreshToken(string token)
    {
        var user = getUserByRefreshToken(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            // revoke all descendant tokens in case this token has been compromised
            revokeDescendantRefreshTokens(refreshToken, user);
            _unitOfWork.Repository<IUserRepository>().Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        if (!refreshToken.IsActive)
            throw new Exception("Invalid token");

        // replace old refresh token with a new one (rotate token)
        var newRefreshToken = rotateRefreshToken(refreshToken);
        user.RefreshTokens.Add(newRefreshToken);

        // remove old refresh tokens from user
        removeOldRefreshTokens(user);

        // save changes to db
        _unitOfWork.Repository<IUserRepository>().Update(user);
        await _unitOfWork.SaveChangesAsync();

        // generate new jwt
        var jwtToken = _tokenManager.GenerateJwtToken(user);

        return Response<LoginResponse>.CreateResponse(new LoginResponse
        {
            Id = user.Id,
            FirstName = user.Firstname,
            LastName = user.Lastname,
            Email = user.Email,
            JwtToken = jwtToken,
            RefreshToken = newRefreshToken.Token
        }, StatusCodes.Status200OK);
    }

    public async Task RevokeToken(string token)
    {
        var user = getUserByRefreshToken(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new Exception("Invalid token");

        // revoke token and save
        revokeRefreshToken(refreshToken, "Revoked without replacement");
        _unitOfWork.Repository<IUserRepository>().Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private User getUserByRefreshToken(string token)
    {
        var user = _unitOfWork.Repository<IUserRepository>().Get(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            throw new Exception("Invalid token");

        return user;
    }

    private RefreshToken rotateRefreshToken(RefreshToken refreshToken)
    {
        var newRefreshToken = _tokenManager.GenerateRefreshToken();
        revokeRefreshToken(refreshToken, newRefreshToken.Token);
        return newRefreshToken;
    }

    private void removeOldRefreshTokens(User user)
    {
        // remove old inactive refresh tokens from user based on TTL in app settings
        user.RefreshTokens.RemoveAll(x =>
            !x.IsActive &&
            x.CreatedDate.GetValueOrDefault().AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private void revokeDescendantRefreshTokens(RefreshToken refreshToken, User user)
    {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken.IsActive)
                revokeRefreshToken(childToken);
            else
                revokeDescendantRefreshTokens(childToken, user);
        }
    }

    private void revokeRefreshToken(RefreshToken token, string replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.ReplacedByToken = replacedByToken;
    }
}
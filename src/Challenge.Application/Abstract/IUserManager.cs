using Challange.Core.Common;
using Challenge.Application.Dto.User.Request;
using Challenge.Application.Dto.User.Response;
using Challenge.Domain;

namespace Challenge.Application.Abstract;

public interface IUserManager
{
    Task<User> GetById(Guid id);
    Task Register(RegisterRequest request);
    Task<Response<LoginResponse>> Login(LoginRequest request);

    Task<Response<LoginResponse>> RefreshToken(string token);
    Task RevokeToken(string token);
}
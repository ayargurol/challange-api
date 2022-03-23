using Challenge.Application.Abstract;
using Challenge.Application.Dto.User.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.Api.Controllers;

[Route("api/[controller]"), ApiController, AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserManager _userManager;

    public AuthController(IUserManager userManager)
    {
        _userManager = userManager;
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await _userManager.Register(request);
        return Ok();
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _userManager.Login(request);
        setTokenCookie(response.Data.RefreshToken);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _userManager.RefreshToken(refreshToken);
        setTokenCookie(response.Data.RefreshToken);
        return Ok(response);
    }

    [HttpPost("revoke-token")]
    public IActionResult RevokeToken(RevokeTokenRequest model)
    {
        // accept refresh token in request body or cookie
        var token = model.Token ?? Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });

        _userManager.RevokeToken(token);
        return Ok(new { message = "Token revoked" });
    }


    private void setTokenCookie(string token)
    {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}
using Challange.Core.Common;
using Challenge.Application.Abstract;
using Microsoft.Extensions.Options;

namespace Challenge.Api.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, IUserManager userManager, ITokenManager tokenManager)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = tokenManager.ValidateJwtToken(token);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = await userManager.GetById(userId.Value);
        }

        await _next(context);
    }
}
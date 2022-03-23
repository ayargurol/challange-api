using System.ComponentModel.DataAnnotations;

namespace Challenge.Application.Dto.User.Request;

public class LoginRequest
{
    public string Email { get; set; }

    public string Password { get; set; }
}
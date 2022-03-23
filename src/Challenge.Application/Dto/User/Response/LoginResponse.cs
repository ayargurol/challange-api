using System.Text.Json.Serialization;

namespace Challenge.Application.Dto.User.Response;

public class LoginResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string JwtToken { get; set; }
    public string Role { get; set; }

    [JsonIgnore] // refresh token is returned in http only cookie
    public string RefreshToken { get; set; }
}
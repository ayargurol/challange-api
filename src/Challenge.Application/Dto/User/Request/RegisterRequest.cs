namespace Challenge.Application.Dto.User.Request;

public class RegisterRequest
{

    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}
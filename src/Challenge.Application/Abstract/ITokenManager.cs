using Challenge.Domain;

namespace Challenge.Application.Abstract;

public interface ITokenManager
{
    public string GenerateJwtToken(User user);
    public Guid? ValidateJwtToken(string token);
    public RefreshToken GenerateRefreshToken();
}
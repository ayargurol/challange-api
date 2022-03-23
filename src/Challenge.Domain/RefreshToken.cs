using Microsoft.EntityFrameworkCore;

namespace Challenge.Domain;

[Owned]
public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }
    public string ReplacedByToken { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public new bool IsActive => !IsRevoked && !IsExpired;
}
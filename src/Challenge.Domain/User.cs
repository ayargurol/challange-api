using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Challenge.Domain;

[Table(nameof(User))]
public class User : BaseEntity
{
    [Required]
    public string Firstname { get; set; }
    [Required]
    public string Lastname { get; set; }
    [Required]
    public string Email { get; set; }
   
    [Required]
    public string Role { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }

    [JsonIgnore] 
    public List<RefreshToken> RefreshTokens { get; set; }
}
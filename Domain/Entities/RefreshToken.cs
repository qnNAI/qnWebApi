using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class RefreshToken {

    [Key]
    public string Token { get; set; } = null!;

    public string JwtId { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
}


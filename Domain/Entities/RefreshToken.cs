using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class RefreshToken {

    [Key]
    public string Token { get; set; } = null!;

    public string JwtId { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Expires { get; set; }

    public bool IsRevoked { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;

    public bool IsActive => !IsRevoked && !IsExpired;


    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
}


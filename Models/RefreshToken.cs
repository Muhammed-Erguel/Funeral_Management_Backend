namespace Funeral_Management_Backend.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    // Wir speichern nicht den echten Token,
    // sondern nur seinen Hash.
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;
}
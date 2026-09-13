namespace Funeral_Management_Backend.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }

    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
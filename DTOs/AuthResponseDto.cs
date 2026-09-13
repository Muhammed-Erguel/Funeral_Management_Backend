namespace Funeral_Management.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;

    public int UserId { get; set; }
    public int CompanyId { get; set; }

    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
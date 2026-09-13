using Funeral_Management.DTOs.Company;

namespace Funeral_Management_Backend.DTOs.Auth;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public CreateCompanyDto Company { get; set; } = new();
}
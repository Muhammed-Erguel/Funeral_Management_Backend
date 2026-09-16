namespace Funeral_Management_Backend.DTOs.Me;

public class MeResponseDto
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public CompanyInfoDto Company { get; set; } = new();
}

public class CompanyInfoDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
using System.Security.Claims;

namespace Funeral_Management_Backend.Services.CurrentUser;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException(
            "No HTTP context available."
        );

    public bool IsAuthenticated =>
        User.Identity?.IsAuthenticated ?? false;

    public int UserId
    {
        get
        {
            var value = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(value, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user ID."
                );
            }

            return userId;
        }
    }

    public int CompanyId
    {
        get
        {
            var value = User.FindFirstValue("company_id");

            if (!int.TryParse(value, out var companyId))
            {
                throw new UnauthorizedAccessException(
                    "Invalid company ID."
                );
            }

            return companyId;
        }
    }

    public string Role
    {
        get
        {
            return User.FindFirstValue(ClaimTypes.Role)
                ?? throw new UnauthorizedAccessException(
                    "Role claim missing."
                );
        }
    }

    public string? Email =>
        User.FindFirstValue(ClaimTypes.Email);
}
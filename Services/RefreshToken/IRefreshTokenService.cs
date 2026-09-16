using Funeral_Management_Backend.Models;

namespace Funeral_Management_Backend.Services.Auth;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(User user);

    Task<User?> ValidateAsync(string refreshToken);

    Task RevokeAsync(string refreshToken);
}
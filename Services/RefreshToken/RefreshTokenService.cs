using System.Security.Cryptography;
using System.Text;
using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;

    public RefreshTokenService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateAsync(User user)
    {
        // 64 zufällige Bytes
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        // Token, der an den Client geschickt wird
        var token = Convert.ToBase64String(randomBytes);

        var tokenHash = HashToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,

            TokenHash = tokenHash,

            CreatedAt = DateTime.UtcNow,

            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync();

        return token;
    }

    public async Task<User?> ValidateAsync(
        string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r =>
                r.TokenHash == tokenHash);

        if (storedToken == null)
            return null;

        if (storedToken.RevokedAt != null)
            return null;

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        return storedToken.User;
    }

    public async Task RevokeAsync(
        string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r =>
                r.TokenHash == tokenHash);

        if (storedToken == null)
            return;

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token)
        );

        return Convert.ToHexString(bytes);
    }
}
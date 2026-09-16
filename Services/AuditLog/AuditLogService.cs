using System.Text.Json;
using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.CurrentUser;

namespace Funeral_Management_Backend.Services.Audit;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditLogService(
        AppDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task LogAsync(
        string action,
        string entityType,
        int? entityId = null,
        int? caseId = null,
        object? metadata = null)
    {
        var auditLog = new AuditLog
        {
            CompanyId = _currentUser.CompanyId,
            UserId = _currentUser.UserId,

            CaseId = caseId,

            Action = action,
            EntityType = entityType,
            EntityId = entityId,

            Metadata = metadata == null
                ? null
                : JsonSerializer.Serialize(metadata),

            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();
    }
}
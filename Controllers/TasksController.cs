using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Task;
using Funeral_Management_Backend.Models;
using Funeral_Management_Backend.Services.CurrentUser;
using Funeral_Management_Backend.Services.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public TasksController(
        AppDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    // GET /api/tasks
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var companyId = _currentUser.CompanyId;

        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.CompanyId == companyId)
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                CaseId = t.CaseId,
                LocationId = t.LocationId,
                Kind = t.Kind,
                Title = t.Title,
                Description = t.Description,
                DueAt = t.DueAt,
                ReminderAt = t.ReminderAt,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(tasks);
    }

    // POST /api/tasks
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var companyId = _currentUser.CompanyId;
        var userId = _currentUser.UserId;

        // Falls ein Case angegeben wurde:
        // prüfen, ob er wirklich zur eigenen Company gehört.
        if (dto.CaseId.HasValue)
        {
            var caseExists = await _context.Cases
                .AnyAsync(c =>
                    c.Id == dto.CaseId.Value &&
                    c.CompanyId == companyId);

            if (!caseExists)
            {
                return BadRequest(new
                {
                    message = "Invalid case."
                });
            }
        }

        // Falls eine Location angegeben wurde:
        // prüfen, ob sie wirklich zur eigenen Company gehört.
        if (dto.LocationId.HasValue)
        {
            var locationExists = await _context.Locations
                .AnyAsync(l =>
                    l.Id == dto.LocationId.Value &&
                    l.CompanyId == companyId);

            if (!locationExists)
            {
                return BadRequest(new
                {
                    message = "Invalid location."
                });
            }
        }

        var task = new TaskItem
        {
            CompanyId = companyId,

            CaseId = dto.CaseId,
            LocationId = dto.LocationId,

            CreatedBy = userId,

            Kind = dto.Kind,
            Title = dto.Title,
            Description = dto.Description,

            DueAt = dto.DueAt,
            ReminderAt = dto.ReminderAt,

            Status = dto.Status,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);

        await _context.SaveChangesAsync();

        await _auditLog.LogAsync(
            action: "TaskCreated",
            entityType: "Task",
            entityId: task.Id,
            caseId: task.CaseId,
            metadata: new
            {
                task.Kind,
                task.Title
            }
        );

        var response = new TaskResponseDto
        {
            Id = task.Id,
            CaseId = task.CaseId,
            LocationId = task.LocationId,
            Kind = task.Kind,
            Title = task.Title,
            Description = task.Description,
            DueAt = task.DueAt,
            ReminderAt = task.ReminderAt,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };

        return Ok(response);
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateTaskDto dto)
    {
        var companyId = _currentUser.CompanyId;

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                t.CompanyId == companyId);

        if (task == null)
            return NotFound();

        // --------------------------------------------------
        // Case prüfen
        // --------------------------------------------------

        if (dto.CaseId.HasValue)
        {
            var caseExists = await _context.Cases
                .AnyAsync(c =>
                    c.Id == dto.CaseId.Value &&
                    c.CompanyId == companyId);

            if (!caseExists)
            {
                return BadRequest(new
                {
                    message = "Invalid case."
                });
            }

            task.CaseId = dto.CaseId;
        }

        // --------------------------------------------------
        // Location prüfen
        // --------------------------------------------------

        if (dto.LocationId.HasValue)
        {
            var locationExists = await _context.Locations
                .AnyAsync(l =>
                    l.Id == dto.LocationId.Value &&
                    l.CompanyId == companyId);

            if (!locationExists)
            {
                return BadRequest(new
                {
                    message = "Invalid location."
                });
            }

            task.LocationId = dto.LocationId;
        }

        // Alte Werte für Audit Log
        var oldStatus = task.Status;
        var oldTitle = task.Title;

        // --------------------------------------------------
        // Task aktualisieren
        // --------------------------------------------------

        if (dto.Kind != null)
            task.Kind = dto.Kind;

        if (dto.Title != null)
            task.Title = dto.Title;

        if (dto.Description != null)
            task.Description = dto.Description;

        if (dto.DueAt.HasValue)
            task.DueAt = dto.DueAt;

        if (dto.ReminderAt.HasValue)
            task.ReminderAt = dto.ReminderAt;

        if (dto.Status != null)
            task.Status = dto.Status;

        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "TaskUpdated",
            entityType: "Task",
            entityId: task.Id,
            caseId: task.CaseId,
            metadata: new
            {
                OldTitle = oldTitle,
                NewTitle = task.Title,
                OldStatus = oldStatus,
                NewStatus = task.Status
            }
        );

        return NoContent();
    }

    // DELETE /api/tasks/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = _currentUser.CompanyId;

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                t.CompanyId == companyId);

        if (task == null)
            return NotFound();

        // --------------------------------------------------
        // Daten für Audit Log vor dem Löschen sichern
        // --------------------------------------------------

        var deletedTask = new
        {
            task.Id,
            task.CaseId,
            task.LocationId,
            task.Kind,
            task.Title,
            task.Description,
            task.DueAt,
            task.ReminderAt,
            task.Status
        };

        var caseId = task.CaseId;

        // --------------------------------------------------
        // Task löschen
        // --------------------------------------------------

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();

        // --------------------------------------------------
        // Audit Log
        // --------------------------------------------------

        await _auditLog.LogAsync(
            action: "TaskDeleted",
            entityType: "Task",
            entityId: id,
            caseId: caseId,
            metadata: deletedTask
        );

        return NoContent();
    }
}
using Funeral_Management_Backend.Data;
using Funeral_Management_Backend.DTOs.Task;
using Funeral_Management_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Controllers;

[ApiController]
[Route("api/companies/{companyId:int}/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int companyId)
    {
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

    [HttpPost]
    public async Task<IActionResult> Create(
        int companyId,
        int userId,
        CreateTaskDto dto)
    {
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

        return Ok(task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int companyId,
        int id,
        UpdateTaskDto dto)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                t.CompanyId == companyId);

        if (task == null)
            return NotFound();

        if (dto.CaseId.HasValue)
            task.CaseId = dto.CaseId;

        if (dto.LocationId.HasValue)
            task.LocationId = dto.LocationId;

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

        return NoContent();
    }
}
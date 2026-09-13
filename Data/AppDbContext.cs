using Funeral_Management_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Funeral_Management_Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<CaseContact> CaseContacts => Set<CaseContact>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<CaseLocation> CaseLocations => Set<CaseLocation>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCompany(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureCase(modelBuilder);
        ConfigureContact(modelBuilder);
        ConfigureCaseContact(modelBuilder);
        ConfigureLocation(modelBuilder);
        ConfigureCaseLocation(modelBuilder);
        ConfigureDocumentType(modelBuilder);
        ConfigureDocument(modelBuilder);
        ConfigureTask(modelBuilder);
        ConfigureAuditLog(modelBuilder);
    }

    private static void ConfigureCompany(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("companies");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .HasColumnName("company_id");

            entity.Property(c => c.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(c => c.Street)
                .HasColumnName("street")
                .HasMaxLength(255);

            entity.Property(c => c.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(20);

            entity.Property(c => c.City)
                .HasColumnName("city")
                .HasMaxLength(100);

            entity.Property(c => c.Country)
                .HasColumnName("country")
                .HasMaxLength(100);

            entity.Property(c => c.Phone)
                .HasColumnName("phone")
                .HasMaxLength(50);

            entity.Property(c => c.Email)
                .HasColumnName("email")
                .HasMaxLength(255);

            entity.Property(c => c.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(c => c.UpdatedAt)
                .HasColumnName("updated_at");
        });
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasColumnName("user_id");

            entity.Property(u => u.CompanyId)
                .HasColumnName("company_id");

            entity.Property(u => u.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            entity.Property(u => u.Role)
                .HasColumnName("role")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.CreatedAt)
                .HasColumnName("created_at");

            entity.HasOne(u => u.Company)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Case>(entity =>
        {
            entity.ToTable("cases");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .HasColumnName("case_id");

            entity.Property(c => c.CompanyId)
                .HasColumnName("company_id");

            entity.Property(c => c.CaseNumber)
                .HasColumnName("case_number")
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(c => new { c.CompanyId, c.CaseNumber })
                .IsUnique();

            entity.Property(c => c.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.DeceasedFirstName)
                .HasColumnName("deceased_first_name")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.DeceasedLastName)
                .HasColumnName("deceased_last_name")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.DateOfBirth)
                .HasColumnName("date_of_birth");

            entity.Property(c => c.DateOfDeath)
                .HasColumnName("date_of_death");

            entity.Property(c => c.DestinationCountry)
                .HasColumnName("destination_country")
                .HasMaxLength(100);

            entity.Property(c => c.DestinationCity)
                .HasColumnName("destination_city")
                .HasMaxLength(100);

            entity.Property(c => c.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(c => c.UpdatedBy)
                .HasColumnName("updated_by");

            entity.Property(c => c.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(c => c.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasOne(c => c.Company)
                .WithMany(company => company.Cases)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedCases)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.UpdatedByUser)
                .WithMany(u => u.UpdatedCases)
                .HasForeignKey(c => c.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("contacts");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id)
                .HasColumnName("contact_id");

            entity.Property(c => c.CompanyId)
                .HasColumnName("company_id");

            entity.Property(c => c.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(100);

            entity.Property(c => c.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(100);

            entity.Property(c => c.CompanyName)
                .HasColumnName("company_name")
                .HasMaxLength(255);

            entity.Property(c => c.Phone)
                .HasColumnName("phone")
                .HasMaxLength(50);

            entity.Property(c => c.Email)
                .HasColumnName("email")
                .HasMaxLength(255);

            entity.Property(c => c.Language)
                .HasColumnName("language")
                .HasMaxLength(50);

            entity.Property(c => c.Notes)
                .HasColumnName("notes");

            entity.Property(c => c.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(c => c.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasOne(c => c.Company)
                .WithMany(company => company.Contacts)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCaseContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CaseContact>(entity =>
        {
            entity.ToTable("case_contacts");

            entity.HasKey(cc => new
            {
                cc.CaseId,
                cc.ContactId,
                cc.Role
            });

            entity.Property(cc => cc.CaseId)
                .HasColumnName("case_id");

            entity.Property(cc => cc.ContactId)
                .HasColumnName("contact_id");

            entity.Property(cc => cc.Role)
                .HasColumnName("role")
                .HasMaxLength(50);

            entity.Property(cc => cc.Notes)
                .HasColumnName("notes");

            entity.HasOne(cc => cc.Case)
                .WithMany(c => c.CaseContacts)
                .HasForeignKey(cc => cc.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cc => cc.Contact)
                .WithMany(c => c.CaseContacts)
                .HasForeignKey(cc => cc.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureLocation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("locations");

            entity.HasKey(l => l.Id);

            entity.Property(l => l.Id)
                .HasColumnName("location_id");

            entity.Property(l => l.CompanyId)
                .HasColumnName("company_id");

            entity.Property(l => l.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(l => l.Type)
                .HasColumnName("type")
                .HasMaxLength(50);

            entity.Property(l => l.Street)
                .HasColumnName("street")
                .HasMaxLength(255);

            entity.Property(l => l.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(20);

            entity.Property(l => l.City)
                .HasColumnName("city")
                .HasMaxLength(100);

            entity.Property(l => l.Country)
                .HasColumnName("country")
                .HasMaxLength(100);

            entity.Property(l => l.Phone)
                .HasColumnName("phone")
                .HasMaxLength(50);

            entity.Property(l => l.Email)
                .HasColumnName("email")
                .HasMaxLength(255);

            entity.Property(l => l.OpeningHours)
                .HasColumnName("opening_hours");

            entity.Property(l => l.Notes)
                .HasColumnName("notes");

            entity.Property(l => l.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(l => l.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasOne(l => l.Company)
                .WithMany(c => c.Locations)
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCaseLocation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CaseLocation>(entity =>
        {
            entity.ToTable("case_locations");

            entity.HasKey(cl => new
            {
                cl.CaseId,
                cl.LocationId,
                cl.Role
            });

            entity.Property(cl => cl.CaseId)
                .HasColumnName("case_id");

            entity.Property(cl => cl.LocationId)
                .HasColumnName("location_id");

            entity.Property(cl => cl.Role)
                .HasColumnName("role")
                .HasMaxLength(50);

            entity.Property(cl => cl.Notes)
                .HasColumnName("notes");

            entity.HasOne(cl => cl.Case)
                .WithMany(c => c.CaseLocations)
                .HasForeignKey(cl => cl.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cl => cl.Location)
                .WithMany(l => l.CaseLocations)
                .HasForeignKey(cl => cl.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureDocumentType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("document_types");

            entity.HasKey(dt => dt.Id);

            entity.Property(dt => dt.Id)
                .HasColumnName("document_type_id");

            entity.Property(dt => dt.CompanyId)
                .HasColumnName("company_id");

            entity.Property(dt => dt.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(dt => dt.IsActive)
                .HasColumnName("is_active");

            entity.Property(dt => dt.RetentionMonths)
                .HasColumnName("retention_months");

            entity.Property(dt => dt.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(dt => dt.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasIndex(dt => new
            {
                dt.CompanyId,
                dt.Name
            }).IsUnique();

            entity.HasOne(dt => dt.Company)
                .WithMany(c => c.DocumentTypes)
                .HasForeignKey(dt => dt.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureDocument(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("documents");

            entity.HasKey(d => d.Id);

            entity.Property(d => d.Id)
                .HasColumnName("document_id");

            entity.Property(d => d.CaseId)
                .HasColumnName("case_id");

            entity.Property(d => d.DocumentTypeId)
                .HasColumnName("document_type_id");

            entity.Property(d => d.UploadedBy)
                .HasColumnName("uploaded_by");

            entity.Property(d => d.StorageKey)
                .HasColumnName("storage_key")
                .IsRequired();

            entity.Property(d => d.OriginalFilename)
                .HasColumnName("original_filename")
                .IsRequired();

            entity.Property(d => d.MimeType)
                .HasColumnName("mime_type")
                .HasMaxLength(255);

            entity.Property(d => d.SizeBytes)
                .HasColumnName("size_bytes");

            entity.Property(d => d.Status)
                .HasColumnName("status")
                .HasMaxLength(50);

            entity.Property(d => d.Deadline)
                .HasColumnName("deadline");

            entity.Property(d => d.ReceivedAt)
                .HasColumnName("received_at");

            entity.Property(d => d.CreatedAt)
                .HasColumnName("created_at");

            entity.HasOne(d => d.Case)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.DocumentType)
                .WithMany(dt => dt.Documents)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.UploadedByUser)
                .WithMany(u => u.UploadedDocuments)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTask(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .HasColumnName("task_id");

            entity.Property(t => t.CompanyId)
                .HasColumnName("company_id");

            entity.Property(t => t.CaseId)
                .HasColumnName("case_id");

            entity.Property(t => t.LocationId)
                .HasColumnName("location_id");

            entity.Property(t => t.CreatedBy)
                .HasColumnName("created_by");

            entity.Property(t => t.Kind)
                .HasColumnName("kind")
                .HasMaxLength(50);

            entity.Property(t => t.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(t => t.Description)
                .HasColumnName("description");

            entity.Property(t => t.DueAt)
                .HasColumnName("due_at");

            entity.Property(t => t.ReminderAt)
                .HasColumnName("reminder_at");

            entity.Property(t => t.Status)
                .HasColumnName("status")
                .HasMaxLength(50);

            entity.Property(t => t.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(t => t.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasOne(t => t.Company)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Case)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CaseId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.Location)
                .WithMany(l => l.Tasks)
                .HasForeignKey(t => t.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .HasColumnName("audit_id");

            entity.Property(a => a.CompanyId)
                .HasColumnName("company_id");

            entity.Property(a => a.UserId)
                .HasColumnName("user_id");

            entity.Property(a => a.CaseId)
                .HasColumnName("case_id");

            entity.Property(a => a.Action)
                .HasColumnName("action")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.EntityType)
                .HasColumnName("entity_type")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.EntityId)
                .HasColumnName("entity_id");

            entity.Property(a => a.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("jsonb");

            entity.Property(a => a.CreatedAt)
                .HasColumnName("created_at");

            entity.HasOne(a => a.Company)
                .WithMany(c => c.AuditLogs)
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Case)
                .WithMany(c => c.AuditLogs)
                .HasForeignKey(a => a.CaseId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
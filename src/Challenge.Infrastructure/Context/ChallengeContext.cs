using System.Reflection;
using Challange.Core.Entities;
using Challenge.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Challenge.Infrastructure.Context;

public class ChallengeContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChallengeContext(DbContextOptions<ChallengeContext> options,IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }



    public override int SaveChanges()
    {
        TrackChanges();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        TrackChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        modelBuilder.HasPostgresExtension("uuid-ossp").Entity<User>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();
        modelBuilder.HasPostgresExtension("uuid-ossp").Entity<Product>().Property(a => a.Id).HasDefaultValueSql("uuid_generate_v4()").ValueGeneratedOnAdd();


        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<User>().HasData(new List<User>()
        //{
        //    new()
        //    {
        //        Id = Guid.Parse("6c4da387-1433-4c7a-adef-bd86fd0e7641"),
        //        Firstname = "Admin",
        //        Lastname = "User",
        //        Role = "Admin",
        //        Email = "admin@challenge.com",
        //        PasswordHash = "$2a$11$isBfD4xTa60SOG5YcCJtR.PaZuhOs3dG4cFfzLqqqi9uRRDcp9niW",
        //        IsActive = true,
        //        IsDeleted = false,
        //        CreatedDate = DateTime.Now
        //    }
        //});

    }

    private void TrackChanges()
    {
        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            if ((entry.Entity is not IAuditableEntity auditable)) continue;

            if (auditable == null) throw new ArgumentNullException(nameof(auditable));

            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyConceptsForAddedEntity(entry);
                    break;
                case EntityState.Modified:
                    ApplyConceptsForUpdatedEntity(entry);
                    break;
                case EntityState.Deleted:
                    ApplyConceptsForDeletedEntity(entry);
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("EntityState type is not defined.");
            }
        }
    }

    protected virtual void ApplyConceptsForAddedEntity(EntityEntry entry)
    {
        if (entry.Entity is not IAuditableEntity)
        {
            return;
        }

        //entry.Reload();
        entry.Entity.As<IAuditableEntity>().IsDeleted = false;
        entry.Entity.As<IAuditableEntity>().IsActive = true;
        entry.Entity.As<IAuditableEntity>().CreatedDate = DateTime.Now;
        entry.Entity.As<IAuditableEntity>().CreatedById = (_httpContextAccessor.HttpContext?.Items["User"] as User)?.Id;
        //entry.State = EntityState.Added;
    }

    protected virtual void ApplyConceptsForUpdatedEntity(EntityEntry entry)
    {
        if (entry.Entity is not IAuditableEntity)
        {
            return;
        }

        //entry.Reload();
        //Entry(auditable).Property(x => x.CreatedDate).IsModified = false;
        entry.Entity.As<IAuditableEntity>().IsDeleted = false;
        entry.Entity.As<IAuditableEntity>().IsActive = true;
        entry.Entity.As<IAuditableEntity>().UpdatedDate = DateTime.Now;
        entry.Entity.As<IAuditableEntity>().UpdatedById = (_httpContextAccessor.HttpContext?.Items["User"] as User)?.Id;
        //entry.State = EntityState.Modified;
    }

    protected virtual void ApplyConceptsForDeletedEntity(EntityEntry entry)
    {
        if (entry.Entity is not IAuditableEntity)
        {
            return;
        }

        entry.Reload();
        entry.Entity.As<IAuditableEntity>().IsDeleted = true;
        entry.Entity.As<IAuditableEntity>().IsActive = false;
        entry.Entity.As<IAuditableEntity>().UpdatedDate = DateTime.Now;
        entry.Entity.As<IAuditableEntity>().UpdatedById = (_httpContextAccessor.HttpContext?.Items["User"] as User)?.Id;
        entry.State = EntityState.Modified;
    }
}
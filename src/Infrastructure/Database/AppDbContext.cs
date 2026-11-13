using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Entities;

namespace Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users {get; set;}
    public DbSet<Folder> Folders {get; set;}
    public DbSet<Project> Projects {get; set;}
    public DbSet<TaskItem> Tasks {get; set;}
    public DbSet<Tag> Tags {get; set;}
    public DbSet<Change> Changes {get; set;}
    public DbSet<LogEntry> LogEntries {get; set;}
    public DbSet<Notification> Notifications {get; set;}
    public DbSet<FolderProjectLink> FolderProjectLinks {get; set;}
    public DbSet<TaskTagLink> TaskTagLinks {get; set;}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.ID)
            .ValueGeneratedNever();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Folder>()
            .Property(f => f.ID)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Folder>()
            .Property(f => f.Name)
            .UseCollation("NOCASE");

        modelBuilder.Entity<Folder>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(f => f.OwnerID)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Folder>()
            .HasIndex(f => new {f.OwnerID, f.Name})
            .IsUnique();
        
        modelBuilder.Entity<Project>()
            .Property(p => p.ID)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Project>()
            .Property(f => f.Name)
            .UseCollation("NOCASE");

        modelBuilder.Entity<Project>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.OwnerID)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Project>()
            .HasIndex(p => new {p.OwnerID, p.Name})
            .IsUnique()
            .HasFilter("RemoteID IS NULL");

        modelBuilder.Entity<Project>()
            .HasIndex(p => new {p.RemoteID, p.AssignedUserID})
            .IsUnique()
            .HasFilter("RemoteID IS NULL");
        
        modelBuilder.Entity<TaskItem>()
            .Property(t => t.ID)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<TaskItem>()
            .Property(f => f.Name)
            .UseCollation("NOCASE");
        
        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => new {t.ProjectID, t.Name})
            .IsUnique();
        
        modelBuilder.Entity<TaskItem>()
            .HasIndex(p => new {p.ProjectID, p.RemoteID})
            .IsUnique();
        
        modelBuilder.Entity<TaskItem>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(t => t.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Tag>()
            .Property(t => t.ID)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Tag>()
            .Property(f => f.Name)
            .UseCollation("NOCASE");

        modelBuilder.Entity<Tag>()
            .HasIndex(t => new {t.ProjectID, t.Name})
            .IsUnique();
        
        modelBuilder.Entity<Tag>()
            .HasIndex(p => new {p.ProjectID, p.RemoteID})
            .IsUnique();
        
        modelBuilder.Entity<Tag>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(t => t.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FolderProjectLink>()
            .HasKey(fpl => new {fpl.FolderID, fpl.ProjectID});
        
        modelBuilder.Entity<FolderProjectLink>()
            .HasOne<Folder>()
            .WithMany()
            .HasForeignKey(fpl => fpl.FolderID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FolderProjectLink>()
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey(fpl => fpl.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<TaskTagLink>()
            .HasKey(ttl => new {ttl.TaskID, ttl.TagID});

        modelBuilder.Entity<TaskTagLink>()
            .HasOne<TaskItem>()
            .WithMany()
            .HasForeignKey(ttl => ttl.TaskID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskTagLink>()
            .HasOne<Tag>()
            .WithMany()
            .HasForeignKey(ttl => ttl.TagID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Change>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserID)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Change>()
            .HasIndex(c => new {c.UserID, c.TargetType, c.TargetID})
            .IsUnique();
    }
}

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
    public DbSet<LocalChange> LocalChanges {get; set;}
    public DbSet<LogEntry> LogEntries {get; set;}
    public DbSet<Notification> Notifications {get; set;}
    public DbSet<FolderProjectLink> FolderProjectLinks {get; set;}
    public DbSet<TaskTagLink> TaskTagLinks {get; set;}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
        
        modelBuilder.Entity<Folder>()
            .HasIndex(f => new {f.OwnerID, f.Name}).IsUnique();
        
        modelBuilder.Entity<Project>()
            .HasIndex(p => new {p.OwnerID, p.Name}).IsUnique();
        
        modelBuilder.Entity<Tag>()
            .HasIndex(t => new {t.ProjectID, t.Name}).IsUnique();

        modelBuilder.Entity<FolderProjectLink>()
            .HasKey(fpl => new {fpl.FolderID, fpl.ProjectID});
        
        modelBuilder.Entity<TaskTagLink>()
            .HasKey(ttl => new {ttl.TaskID, ttl.TagID});
    }
}

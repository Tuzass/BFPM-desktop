using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContextFactory()
    {
        string dbPath = GetDatabasePath();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(string.Concat("Data Source=", dbPath))
            .Options;
        
        var context = new AppDbContext(options);
        context.Database.Migrate();
    }

    static private string GetDatabasePath()
    {
        string dbFileName = "data.db";
        string appFolder = "BFPM";
        string localApplicationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string dbPath = Path.Combine(localApplicationDirectory, appFolder);
        Directory.CreateDirectory(dbPath);

        return Path.Combine(dbPath, dbFileName);
    }

    static public AppDbContext CreateContext()
    {
        string dbPath = GetDatabasePath();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(string.Concat("Data Source=", dbPath))
            .Options;
        
        var context = new AppDbContext(options);
        return context;
    }

    public AppDbContext CreateDbContext(string[] args)
    {
        return CreateContext();
    }
}

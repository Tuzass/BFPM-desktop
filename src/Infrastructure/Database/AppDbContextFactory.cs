using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContextFactory
{
    static private string GetDatabasePath()
    {
        string dbFileName = "bfpm.db";
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
        context.Database.EnsureCreated();
        return context;
    }
}

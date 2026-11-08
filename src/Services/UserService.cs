using Domain.Entities;
using Services.Exceptions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Services;

static public class UserService
{
    public static void CreateLocalUser()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool alreadyExists = context.Users
                .Any(u => u.ID == 0);

            if (alreadyExists)
            {
                return;
            }

            User localUser = new(0, "Local User", "");
            context.Add(localUser);
            context.SaveChanges();
        }
    }

    public static async Task StoreOrUpdate(int userID, string username, string email)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);
            
            if (user is null)
            {
                user = new(userID, username, email);
                context.Users.Add(user);
            }

            else
            {
                user.Username = username;
                user.Email = email;
            }
            
            await context.SaveChangesAsync();
        }
    }

    public static async Task UpdateInformation(int userID, string username, string email)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);
            
            if (user is null)
            {
                throw new UserNotRegisteredLocallyException();
            }
            
            user.Username = username;
            user.Email = email;
            await context.SaveChangesAsync();
        }
    }

    public static async Task<User> GetCurrentUser()
    {
        var settings = SettingsService.GetSettings();

        if (!settings.TryGetValue("CurrentUserID", out string? idString))
        {
            settings.Add("CurrentUserID", "");
            SettingsService.UpdateSettings(settings);
        }

        if (!int.TryParse(idString, out int currentUserID))
        {
            throw new NoCurrentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == currentUserID);
            
            if (user is null)
            {
                throw new NoCurrentUserException();
            }

            return user;
        }
    }

    public static async Task<List<User>> GetUsers()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Users
                .ToListAsync();
        }
    }

    public static async Task<User> GetLocalUser()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Users
                .AsNoTracking()
                .SingleAsync(u => u.ID == 0);
        }
    }

    public static async Task<List<User>> GetStoredUsers()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Users
                .Where(u => u.ID > 0)
                .ToListAsync();
        }
    }

    public static async Task RemoveRegisteredUser(int userID)
    {
        if (userID == 0) return;

        using (var context = AppDbContextFactory.CreateContext())
        {
            var currentUser = await GetCurrentUser();

            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);

            if (user is null)
            {
                throw new UserNotRegisteredLocallyException();
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SwitchCurrentUser(int? newCurrentUserID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            if (newCurrentUserID is not null)
            {
                User? newCurrentUser = await context.Users
                    .Where(u => u.ID == newCurrentUserID.Value)
                    .SingleOrDefaultAsync();
                
                if (newCurrentUser is null)
                {
                    throw new UserNotRegisteredLocallyException();
                }
            }

            var settings = SettingsService.GetSettings();
            settings["CurrentUserID"] = newCurrentUserID is null ? "" : newCurrentUserID.Value.ToString();
            SettingsService.UpdateSettings(settings);
        }
    }
}

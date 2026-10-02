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

    public static async Task<bool> ValidateUser(int userID)
    {
        // checks if a user is registered and active in the remote DB
        // TODO implement after ApiClient is done
        return true;
    }

    public static void Register()
    {
        // gathers information and sends it to the API to be added as a new user
        // TODO: implement after ApiClient is done
    }

    public static void Login()
    {
        // gathers information and sends it to the API as part of the authentication process
        // TODO: implement after ApiClient is done
    }

    public static void Logoff()
    {
        // switches to "no current user"
        // deletes whatever authentication tokens are stored for a given user
        // TODO: implement after ApiClient is done
    }

    public static void ChangeUsername()
    {
        // TODO: implement after ApiClient is done
    }

    public static void ChangeEmail()
    {
        // TODO: implement after ApiClient is done
    }

    public static void ChangePassword()
    {
        // TODO: implement after ApiClient is done
    }

    public static async Task<User?> GetRegisteredUser(int userID)
    {
        // retrieves a user's information from the local DB
        // if not present, retrieves it from the remote DB
        // TODO: implement after ApiClient is done
        return null;
    }

    public static async Task DeleteUser(int userID)
    {
        // marks the user as 'inactive' in the remote DB
        // TODO: implement after ApiClient is done

        await RemoveRegisteredUser(userID);
    }

    public static async Task<User?> GetCurrentUser()
    {
        var settings = SettingsService.GetSettings();

        if (!settings.TryGetValue("CurrentUserID", out string? idString))
        {
            settings.Add("CurrentUserID", "");
            SettingsService.UpdateSettings(settings);
        }

        if (!int.TryParse(idString, out int currentUserID))
        {
            return null;
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Users
                .SingleOrDefaultAsync(u => u.ID == currentUserID);
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

    public static async Task<List<User>> GetStoredRegisteredUsers()
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

            if (currentUser is not null && currentUser.ID == userID)
            {
                Logoff();
            }

            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);

            if (user is null)
            {
                throw new UserNotRegisteredLocally();
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SwitchCurrentUser(int newCurrentUserID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            User? newCurrentUser = await context.Users
                .Where(u => u.ID == newCurrentUserID)
                .SingleOrDefaultAsync();
            
            if (newCurrentUser is null)
            {
                throw new UserNotRegisteredLocally();
            }

            var settings = SettingsService.GetSettings();
            settings["CurrentUserID"] = newCurrentUserID.ToString();
            SettingsService.UpdateSettings(settings);
        }
    }
}

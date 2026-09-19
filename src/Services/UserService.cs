using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Services;

static public class UserService
{
    public static void ValidateUser()
    {
        // checks if a user is registered and active in the remote DB
        // TODO implement after ApiClient is done
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

    public static void GetRegisteredUser()
    {
        // retrieves a user's information from the local DB
        // if not present, retrieves it from the remote DB
        // TODO: implement after ApiClient is done
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
            var user = await context.Users
                .SingleAsync(u => u.ID == userID);

            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    public static void SwitchCurrentUser(int newCurrentUserID)
    {
        var settings = SettingsService.GetSettings();
        settings["CurrentUserID"] = newCurrentUserID.ToString();
        SettingsService.UpdateSettings(settings);
    }
}

namespace Services;

static public class SettingsService
{
    static private readonly Dictionary<string, string> DefaultSettings = new() {
        {"CurrentUserID", "0"},
        {"OsLevelNotifications", "true"}
    };

    static private string GetSettingsPath()
    {
        string settingsFileName = "settings.ini";
        string appFolder = "BFPM";
        string localApplicationDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string settingsPath = Path.Combine(localApplicationDirectory, appFolder);
        Directory.CreateDirectory(settingsPath);

        return Path.Combine(settingsPath, settingsFileName);
    }

    static public Dictionary<string, string> GetSettings()
    {
        string settingsPath = GetSettingsPath();
        string[] settingsStrings = File.ReadAllLines(settingsPath);
        Dictionary<string, string> settings = [];
        
        foreach (var s in settingsStrings)
        {
            int equalsPosition = s.IndexOf('=');
            if (equalsPosition == -1)
            {
                continue;
            }

            string key = s[..equalsPosition];
            string value = s[(equalsPosition + 1)..];
            settings.Add(key, value);
        }

        return settings;
    }

    static public void UpdateSettings(Dictionary<string, string> newSettings)
    {
        string settingsPath = GetSettingsPath();
        string[] newSettingsStrings = new string[newSettings.Count];

        int i = 0;
        foreach (var entry in newSettings)
        {
            newSettingsStrings[i] = string.Concat(entry.Key, '=', entry.Value);
            i++;
        }

        File.WriteAllLines(settingsPath, newSettingsStrings);
    }

    static public void CreateDefaultSettings()
    {
        UpdateSettings(DefaultSettings);
    }
}

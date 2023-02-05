using System;
using System.Text.Json;
using System.IO;

namespace ebay_api_inventory.Entities;
class AppSettings
{
    public string ClientIdDev { get; set; } = @"";
    public string DeveloperIdDev { get; set; } = @"";
    public string DeveloperSecretDev { get; set; } = @"";

    public string ClientIdProd { get; set; } = @"";
    public string DeveloperIdProd { get; set; } = @"";
    public string DeveloperSecretProd { get; set; } = @"";
    public string RedirectUriDev { get; set; } = @"";
    public string RedirectUriProd { get; set; } = @"";

    public static AppSettings Get() 
    {
        string currWrkDir = Directory.GetCurrentDirectory();
        string settingsPath = Path.Combine(currWrkDir, "Settings.json"); 
        string settings = File.ReadAllText(settingsPath);

        var possibleSettings = JsonSerializer.Deserialize<AppSettings>(settings);
        if (possibleSettings != null)
        {
            return possibleSettings;
        }

        throw new NullReferenceException(@"Settings.json file not found, please create one to use app.");
    }
}

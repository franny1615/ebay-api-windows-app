using ebay_api_inventory.Entities;
using ebay_api_inventory.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ebay_api_inventory.Network;

class AuthToken
{
    public async Task<UserAccessToken?> ExchangeAsync(string accessToken)
    {
        AppSettings settings = AppSettings.Get();
        eBaySystem ebaySystem = (eBaySystem) UserSettings.Default.System;
        string url = ExchangeUrl(ebaySystem);
        string redirectUrl = RedirectUrl(ebaySystem, settings);
        string base64Auth = BasicAuthorization(settings, ebaySystem);

        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", accessToken },
            { "redirect_uri",  redirectUrl}
        };

        HttpRequestMessage request = new HttpRequestMessage();
        request.Headers.Add("Authorization", "Basic " + base64Auth);
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(url);
        request.Version = HttpVersion.Version20;
        request.Content = new FormUrlEncodedContent(bodyParameters);

        var response = await App.requestClient.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<UserAccessToken>(responseString);
    }

    private string ExchangeUrl(eBaySystem ebaySystem)
    {
        string endpoint = "/identity/v1/oauth2/token";
        if (ebaySystem == eBaySystem.Sandbox)
        {
            return Endpoint.DevelopmentREST.Value + endpoint;
        }

        return Endpoint.ProductionREST.Value + endpoint;
    }

    private string RedirectUrl(eBaySystem ebaySystem, AppSettings settings)
    {
        if (ebaySystem == eBaySystem.Sandbox)
        {
            return settings.RedirectUriDev;
        }

        return settings.RedirectUriProd;
    }

    private string BasicAuthorization(AppSettings settings, eBaySystem ebaySystem)
    {
        if (ebaySystem == eBaySystem.Sandbox)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.ClientIdDev + ":" + settings.DeveloperSecretDev));
        }

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.ClientIdProd + ":" + settings.DeveloperSecretProd));
    }
}

using ebay_api_inventory.Database;
using ebay_api_inventory.Entities;
using ebay_api_inventory.Network;
using ebay_api_inventory.Utilities;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;

namespace ebay_api_inventory.Main.Pages.Login;

public class LoginViewModel
{
    private DatabaseManager dbManager = new DatabaseManager();
    private UserAccessToken? token;
    private string consentToken = string.Empty;
    private EventHandler<UserAccessToken> loggedIn;

    public LoginViewModel(EventHandler<UserAccessToken> loggedIn)
    {
        this.loggedIn = loggedIn;
        token = dbManager.getUserAccessToken();
    }

    public string OAuthUrl()
    {
        AppSettings settings = AppSettings.Get();
        eBaySystem ebaySystem = (eBaySystem)UserSettings.Default.System;
        string url = @"";
        switch (ebaySystem)
        {
            case eBaySystem.Sandbox:
                url = Endpoint.AuthDevelopmentREST.Value +
                    @"?client_id=" + settings.ClientIdDev +
                    @"&redirect_uri=" + settings.RedirectUriDev +
                    @"&response_type=code" +
                    @"&scope=https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/sell.inventory.readonly";
                break;
            case eBaySystem.Production:
                url = Endpoint.AuthProductionREST.Value +
                    @"?client_id=" + settings.ClientIdProd +
                    @"&redirect_uri=" + settings.RedirectUriProd +
                    @"&response_type=code" +
                    @"&scope=https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/sell.inventory.readonly";
                break;
            default:
                break;
        }

        return url;
    }

    public bool IsError(CoreWebView2WebResourceRequest uriToCheck)
    {
        if (uriToCheck.Uri.Contains("https://signin.ebay.com/"))
        {
            string? error = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("error");
            if (error != null && error == "access_denied")
            {
                return true;
            }
        }

        return false;
    }

    public bool ShouldNavigateToOAuth()
    {
        if (token != null)
        {
            int nowInSeconds = (int)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds;
            int secondsElapsedSinceRefresh = nowInSeconds - token.insertedAtInSeconds;
            if (secondsElapsedSinceRefresh < token.refresh_token_expires_in) 
            {
                loggedIn.Invoke(null, token);
                return false;
            }
        }

        return true;
    }

    public bool IsLoggedIn(CoreWebView2WebResourceRequest uriToCheck)
    {
        if (uriToCheck.Uri.Contains("https://signin.ebay.com/"))
        {
            string? code = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("code");
            string? expiresIn = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("expires_in");

            if (code != null && expiresIn != null)
            {
                consentToken = code;
                return true;
            }
        }

        return false;
    }

    public void ExchangeConsentTokenForUserAccessToken()
    {
        Task.Run(() =>
        {
            try
            {
                AuthToken authToken = new AuthToken();
                UserAccessToken? userAccessToken = authToken.ExchangeAsync(consentToken, grantType: "authorization_code").Result;
                if (userAccessToken != null && userAccessToken.access_token != "")
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        userAccessToken.insertedAtInSeconds = (int) DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds;
                        dbManager.insertAccessToken(userAccessToken);
                        loggedIn.Invoke(null, userAccessToken);
                    }));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        });
    }
}

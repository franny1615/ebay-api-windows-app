using ebay_api_inventory.Entities;
using ebay_api_inventory.Network;
using ebay_api_inventory.Utilities;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ebay_api_inventory.Main.ViewModels;

public class LoginViewModel
{
    private string consentToken = string.Empty;
    private EventHandler<UserAccessToken> loggedIn;
    

    public LoginViewModel(EventHandler<UserAccessToken> loggedIn)
    {
        this.loggedIn = loggedIn;
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
            if(error != null && error == "access_denied")
            {
                return true;
            }
        }

        return false;
    }
    
    public bool IsLoggedIn(CoreWebView2WebResourceRequest uriToCheck) 
    {
        if (uriToCheck.Uri.Contains("https://signin.ebay.com/"))
        {
            string? code = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("code");
            string? expiresIn = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("expires_in");

            if (code != null && expiresIn != null)
            {
                this.consentToken = code;
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
                UserAccessToken? userAccessToken = authToken.ExchangeAsync(this.consentToken).Result;
                if (userAccessToken != null && userAccessToken.access_token != "")
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
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

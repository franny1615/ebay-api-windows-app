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

public class LoginViewModel: ViewModelBase
{
    private UserAccessToken userAccessToken
    {
        get { return userAccessToken; }
        set { userAccessToken = value; OnPropertyChanged(userAccessToken.access_token); }
    }

    private string error
    {
        get { return error; }
        set { error = value; OnPropertyChanged(error); }
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
        bool result = false;
        if (uriToCheck.Uri.Contains("https://signin.ebay.com/"))
        {
            string? error = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("error");
            if(error != null && error == "access_denied")
            {
                result = true;
            }
        }

        return result;
    }
    
    public void CheckIfLoggedIn(CoreWebView2WebResourceRequest uriToCheck) 
    {
        if (uriToCheck.Uri.Contains("https://signin.ebay.com/"))
        {
            string? code = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("code");
            string? expiresIn = HttpUtility.ParseQueryString(uriToCheck.Uri).Get("expires_in");

            if (code != null && expiresIn != null)
            {
                AuthToken authToken = new AuthToken();
                UserAccessToken? userAccessToken = authToken.ExchangeAsync(code).Result;
                if (userAccessToken != null && userAccessToken.access_token != "")
                {
                    this.userAccessToken = userAccessToken;
                }
            }
        }
    }
}

using ebay_api_inventory.Entities;
using ebay_api_inventory.Utilities;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Web.WebView2.Core;
using System.Web;
using System.Threading;
using ebay_api_inventory.Network;

namespace ebay_api_inventory.Main.Pages;
public partial class Login : Page
{
    public Login()
    {
        InitializeComponent();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await WebView.EnsureCoreWebView2Async(null);
        WebView.CoreWebView2.WebResourceResponseReceived += WebResourceResponseReceived;
        NavigateToOAuth();
    }

    private void NavigateToOAuth()
    {
        WebView.CoreWebView2.Profile.ClearBrowsingDataAsync();
        AppSettings settings = AppSettings.Get();
        eBaySystem ebaySystem = (eBaySystem) UserSettings.Default.System;
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

        WebView.Source = new System.Uri(url);
    }

    private void WebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
    {
        if(e.Request.Uri.Contains("https://signin.ebay.com/"))
        {
            string? code = HttpUtility.ParseQueryString(e.Request.Uri).Get("code");
            string? expiresIn = HttpUtility.ParseQueryString(e.Request.Uri).Get("expires_in");
            string? error = HttpUtility.ParseQueryString(e.Request.Uri).Get("error");

            if (error != null)
            {
                NavigateToOAuth();
            }

            if (code != null && expiresIn != null)
            {
                var authToken = new AuthToken();
                var userAccessToken = authToken.ExchangeAsync(code);
            }
        }   
    }
}
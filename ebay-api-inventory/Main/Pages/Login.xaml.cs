using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using ebay_api_inventory.Main.ViewModels;
using System;
using System.Threading;

namespace ebay_api_inventory.Main.Pages;
public partial class Login : Page
{
    private LoginViewModel loginViewModel;

    public Login(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        this.loginViewModel = loginViewModel;
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
        WebView.CoreWebView2.CookieManager.DeleteAllCookies();
        WebView.Source = new Uri(loginViewModel.OAuthUrl());
    }

    private void WebResourceResponseReceived(object? sender, CoreWebView2WebResourceResponseReceivedEventArgs e)
    {
        if (loginViewModel.IsError(e.Request))
        {
            NavigateToOAuth();
        }
        else if (loginViewModel.IsLoggedIn(e.Request))
        {
            NavigateToOAuth();
            loginViewModel.ExchangeConsentTokenForUserAccessToken();
        }
    }
}
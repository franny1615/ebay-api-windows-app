using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using System;

namespace ebay_api_inventory.Main.Pages.Login;
public partial class LoginPage : Page
{
    private LoginViewModel loginViewModel;

    public LoginPage(LoginViewModel loginViewModel)
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
        if (loginViewModel.ShouldNavigateToOAuth())
        {
            WebView.CoreWebView2.CookieManager.DeleteAllCookies();
            WebView.Source = new Uri(loginViewModel.OAuthUrl());
        }
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
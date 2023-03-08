using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ebay_api_inventory.Entities;
using ebay_api_inventory.Main.Pages.Dashboard;
using ebay_api_inventory.Main.Pages.Login;
using ebay_api_inventory.Main.Pages.Settings;
using ebay_api_inventory.Network;

namespace ebay_api_inventory.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoginButton.IsSelected = true;
    }

    private void SideBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = SideBar.SelectedItem as NavButton;
        if (selected == LoginButton)
        {
            LoginViewModel loginVM = new LoginViewModel(loggedIn: UserLoggedIn);
            LoginPage loginPage = new LoginPage(loginVM);
            NavFrame.Navigate(loginPage);
        }
        else if (selected == SettingsButton)
        {
            SettingsPage settingsPage = new SettingsPage();
            NavFrame.Navigate(settingsPage);
        }
    }

    private void UserLoggedIn(object? sender, UserAccessToken userAccessToken)
    {
        long nowInSeconds = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
        long secondsElapsedSinceRefresh = nowInSeconds - userAccessToken.insertedAtInSeconds;
        if (secondsElapsedSinceRefresh > userAccessToken.expires_in)
        {
            AuthToken tokenRequest = new AuthToken();
            UserAccessToken? refreshedToken = tokenRequest.ExchangeAsync(userAccessToken.refresh_token, "").Result;
            if (refreshedToken != null)
            {
                NavigateToDashboard(refreshedToken);
            }
        }
        else
        {
            NavigateToDashboard(userAccessToken);
        }
    }

    private void NavigateToDashboard(UserAccessToken token)
    {
        DashboardViewModel dashboardVM = new DashboardViewModel(token);
        DashboardPage dashboardPage = new DashboardPage(dashboardVM);
        NavFrame.Navigate(dashboardPage);
    }

    private void Navigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode == NavigationMode.Back) { e.Cancel = true; }
    }
}

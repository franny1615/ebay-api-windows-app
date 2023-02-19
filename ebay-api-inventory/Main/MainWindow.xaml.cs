using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ebay_api_inventory.Entities;
using ebay_api_inventory.Main.Pages.Dashboard;
using ebay_api_inventory.Main.Pages.Login;
using ebay_api_inventory.Main.Pages.Settings;

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
        DashboardViewModel dashboardVM = new DashboardViewModel(userAccessToken);
        DashboardPage dashboardPage = new DashboardPage(dashboardVM);
        NavFrame.Navigate(dashboardPage);
    }
}

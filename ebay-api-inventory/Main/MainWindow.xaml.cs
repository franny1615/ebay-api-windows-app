using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ebay_api_inventory.Entities;
using ebay_api_inventory.Main.Pages.Login;
using ebay_api_inventory.Main.Pages.Settings;

namespace ebay_api_inventory.Main;

public partial class MainWindow : Window
{
    private MainViewModel mainViewModel;
    private LoginViewModel loginViewModel; 

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        this.mainViewModel = mainViewModel;
        loginViewModel = new LoginViewModel(loggedIn: UserLoggedIn);
        LoginButton.IsSelected = true;
    }

    private void SideBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = SideBar.SelectedItem as NavButton;
        if (selected == LoginButton)
        {
            LoginPage loginPage = new LoginPage(loginViewModel);
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
        Debug.WriteLine($"User Access Token: {userAccessToken.access_token}");
    }
}

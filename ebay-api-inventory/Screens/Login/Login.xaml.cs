using ebay_api_inventory.Screens.Settings;
using ebay_api_inventory.Entities;
using ebay_api_inventory.Utilities;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ebay_api_inventory;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NavigateToOAuth()
    {
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

        WebBrowser.Navigate(url);
    }

    private void SignInButton_Clicked(object sender, RoutedEventArgs e)
    {
        WebBrowser.Visibility = Visibility.Visible;
        SignInButton.Visibility = Visibility.Hidden;
        NavigateToOAuth();
    }

    private void SettingsButton_Clicked(object sender, RoutedEventArgs e)
    {
        Settings settingsWindow = new Settings();
        settingsWindow.ShowDialog();
    }

    private void Browser_FinishedNavigating(object sender, NavigationEventArgs e)
    {

    }
}
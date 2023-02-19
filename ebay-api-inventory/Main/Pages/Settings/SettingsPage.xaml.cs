using System.Windows;
using System.Windows.Controls;

namespace ebay_api_inventory.Main.Pages.Settings;

public partial class SettingsPage : Page
{
    private eBaySystem ebaySystem;
    public SettingsPage()
    {
        InitializeComponent();
        ebaySystem = (eBaySystem)UserSettings.Default.System;
        InitializeRelevantRadioButton();
    }

    private void InitializeRelevantRadioButton()
    {
        switch (ebaySystem)
        {
            case eBaySystem.Sandbox:
                SandboxRadioButton.IsChecked = true;
                break;
            case eBaySystem.Production:
                ProductionRadioButton.IsChecked = true;
                break;
            default:
                break;
        }
    }

    private void SandboxRadioButton_Clicked(object sender, RoutedEventArgs eventArgs)
    {
        ProductionRadioButton.IsChecked = false;
        ebaySystem = eBaySystem.Sandbox;
        SaveToDefaults();
    }

    private void ProductionRadioButton_Clicked(object sender, RoutedEventArgs eventArgs)
    {
        SandboxRadioButton.IsChecked = false;
        ebaySystem = eBaySystem.Production;
        SaveToDefaults();
    }

    private void SaveToDefaults()
    {
        UserSettings.Default.System = (int)ebaySystem;
        UserSettings.Default.Save();
    }
}

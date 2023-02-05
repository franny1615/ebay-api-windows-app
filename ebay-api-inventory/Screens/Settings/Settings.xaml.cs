using System.Windows;

namespace ebay_api_inventory.Screens.Settings;
public partial class Settings : Window
{
    private eBaySystem ebaySystem; 

    public Settings()
    {
        InitializeComponent();
        ebaySystem = (eBaySystem) UserSettings.Default.System;
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
    }

    private void ProductionRadioButton_Clicked(object sender, RoutedEventArgs eventArgs)
    {
        SandboxRadioButton.IsChecked = false;
        ebaySystem = eBaySystem.Production;
    }

    private void SaveButton_Clicked(object sender, RoutedEventArgs eventArgs)
    {
        UserSettings.Default.System = (int) ebaySystem;
        UserSettings.Default.Save();
        Close();
    }
}

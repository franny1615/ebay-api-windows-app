using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ebay_api_inventory.Main.Pages;

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

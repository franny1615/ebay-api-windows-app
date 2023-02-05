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
using System.Windows.Shapes;

namespace ebay_api_inventory.Screens.Settings;
public partial class Settings : Window
{
    public Settings()
    {
        InitializeComponent();
    }

    private void SandboxRadioButton_Clicked(object sender, RoutedEventArgs eventArgs)
    {
        ProductionRadioButton.IsChecked = false;
    }

    private void ProductionRadioButton_Clicked(object sender, RoutedEventArgs eventArgs)
    {
        SandboxRadioButton.IsChecked = false;
    }

    private void SaveButton_Clicked(object sender, RoutedEventArgs eventArgs)
    {
        // TODO: save to app settings which one they clicked
    }
}

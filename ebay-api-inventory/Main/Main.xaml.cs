using ebay_api_inventory.Main;
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

namespace ebay_api_inventory.Screens.Main;

public partial class Main : Window
{
    public Main()
    {
        InitializeComponent();
    }

    private void SideBar_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = SideBar.SelectedItem as NavButton;
        NavFrame.Navigate(selected.NavLink);
    }
}

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

namespace ebay_api_inventory.Main.Pages.Dashboard;

public partial class DashboardPage : Page
{
    private DashboardViewModel dashboardViewModel;

    public DashboardPage(DashboardViewModel dashboardViewModel)
    {
        InitializeComponent();
        this.dashboardViewModel = dashboardViewModel;
        this.dashboardViewModel.GetMyEbaySelling(
            pageNumber: 1, 
            completion: (errorMessage) =>
        {
            if(errorMessage != null)
            {
                MessageBox.Show(errorMessage);
                return;
            }

            this.DataContext = this.dashboardViewModel;
        });
    }
}

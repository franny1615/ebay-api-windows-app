using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            ListingTable.CanUserSortColumns = false;
            ListingTable.ItemsSource = dashboardViewModel.listings;
        });
    }

    private void ScrollListingTable(object sender, MouseWheelEventArgs e)
    {
        ListingScrollViewer.ScrollToVerticalOffset(ListingScrollViewer.VerticalOffset - e.Delta);
    }

    private void FinishedEditOnListingTable(object sender, DataGridCellEditEndingEventArgs e)
    {
        if(e.EditAction == DataGridEditAction.Commit)
        {
            var column = e.Column as DataGridBoundColumn;
            string bindingPath = (column?.Binding as Binding)?.Path.Path ?? "";
            int rowIndex = e.Row.GetIndex();
            var element = e.EditingElement as TextBox;
            if (bindingPath == "storageLocation")
            {
                dashboardViewModel.updateStorageLocationForListingAt(rowIndex, element?.Text ?? "");
            }
        }
    }
}

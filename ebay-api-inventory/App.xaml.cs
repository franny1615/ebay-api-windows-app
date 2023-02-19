using System.Net.Http;
using System.Windows;
using ebay_api_inventory.Main;

namespace ebay_api_inventory;

public partial class App : Application
{
    public static readonly HttpClient requestClient = new HttpClient();

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow();
        MainWindow.Show();
        base.OnStartup(e);
    }
}

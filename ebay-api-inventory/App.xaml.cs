using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using ebay_api_inventory.Main;
using ebay_api_inventory.Main.ViewModels;

namespace ebay_api_inventory;

public partial class App : Application
{
    public static readonly HttpClient requestClient = new HttpClient();

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow(mainViewModel: new MainViewModel());
        MainWindow.Show();
        base.OnStartup(e);
    }
}

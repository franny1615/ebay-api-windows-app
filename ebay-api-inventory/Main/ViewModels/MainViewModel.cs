using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ebay_api_inventory.Main.ViewModels;

public class MainViewModel: ViewModelBase
{
    public LoginViewModel loginViewModel; 

    public MainViewModel() 
    {
        loginViewModel = new LoginViewModel();
    }
}

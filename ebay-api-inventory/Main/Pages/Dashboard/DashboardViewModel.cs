using ebay_api_inventory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebay_api_inventory.Main.Pages.Dashboard;

public class DashboardViewModel
{
    private UserAccessToken userAccessToken;

    public DashboardViewModel(UserAccessToken userAccessToken) 
    { 
        this.userAccessToken = userAccessToken;
    }
}

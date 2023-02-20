using ebay_api_inventory.Entities;
using ebay_api_inventory.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebay_api_inventory.Main.Pages.Dashboard;

public class DashboardViewModel
{
    private int entriesPerPage = 200;
    private MyEbaySelling myEbaySellingRequest;
    private UserAccessToken userAccessToken;

    public DashboardViewModel(UserAccessToken userAccessToken) 
    { 
        this.userAccessToken = userAccessToken;
        myEbaySellingRequest = new MyEbaySelling();
    }

    public void GetMyEbaySelling(int pageNumber)
    {
        Task.Run(() =>
        {
            try
            {
                List<eBayListing> listings = myEbaySellingRequest.Get(
                        userAccessToken,
                        entriesPerPage: entriesPerPage,
                        pageNumber: pageNumber).Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        });
    }
}

using ebay_api_inventory.Entities;
using ebay_api_inventory.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public ObservableCollection<eBayListing> listings { get; set; }

    public DashboardViewModel(UserAccessToken userAccessToken) 
    { 
        this.userAccessToken = userAccessToken;
        myEbaySellingRequest = new MyEbaySelling();
        listings = new ObservableCollection<eBayListing>();
    }

    public void GetMyEbaySelling(int pageNumber, Action<string?> completion)
    {
        Task.Run(() =>
        {
            try
            {
                var result = myEbaySellingRequest.Get(
                        userAccessToken,
                        entriesPerPage: entriesPerPage,
                        pageNumber: pageNumber).Result;
                result.Sort(delegate (eBayListing a, eBayListing b)
                {
                    return a.CompareTo(b);
                });

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        listings.Add(result[i]);
                    }
                    completion(null);
                }));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                completion(e.Message);
            }
        });
    }
}

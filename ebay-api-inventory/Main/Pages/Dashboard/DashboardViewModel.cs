using ebay_api_inventory.Database;
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
    private DatabaseManager dbManager;
    private int entriesPerPage = 200;
    private MyEbaySelling myEbaySellingRequest;
    private UserAccessToken userAccessToken;

    public ObservableCollection<eBayListing> listings { get; set; }

    public DashboardViewModel(UserAccessToken userAccessToken) 
    { 
        this.userAccessToken = userAccessToken;
        myEbaySellingRequest = new MyEbaySelling();
        listings = new ObservableCollection<eBayListing>();
        dbManager = new DatabaseManager();
    }

    public void GetMyEbaySelling(int pageNumber, Action<string?> completion)
    {
        Task.Run(() =>
        {
            try
            {
                // network fetch
                var result = myEbaySellingRequest.Get(
                        userAccessToken,
                        entriesPerPage: entriesPerPage,
                        pageNumber: pageNumber).Result;
                // local db sync
                var syncedInventory = dbManager.syncInventoryWith(inventory: result);
                // sort by startTime where oldest appear at top.
                syncedInventory.Sort(delegate (eBayListing a, eBayListing b)
                {
                    return a.CompareTo(b);
                });
                // update UI
                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    for (int i = 0; i < syncedInventory.Count; i++)
                    {
                        listings.Add(syncedInventory[i]);
                    }
                    completion(null);
                }));
            }
            catch (Exception e)
            {
                completion(e.Message);
            }
        });
    }

    public void updateStorageLocationForListingAt(int index, string newStorageLocation)
    {
        eBayListing listingToUpdate = listings[index];
        listingToUpdate.storageLocation = newStorageLocation;
        dbManager.updatedStorageLocationFor(listingToUpdate);
    }
}

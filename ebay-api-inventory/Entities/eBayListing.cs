using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebay_api_inventory.Entities;

public class eBayListing
{
    public int bestOfferCount { get; set; }
    public double buyItNowPrice { get; set; }
    public string currencyType { get; set; } = "";
    public string ebayNotes { get; set; } = "";
    public string itemId { get; set; } = "";
    public int quantity { get; set; }
    public int availableQuantity { get; set; }
    public string listingType { get; set; } = "";
    public string title { get; set; } = "";
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebay_api_inventory.Entities;

public class eBayListing: IEquatable<eBayListing>, IComparable<eBayListing>
{
    public int bestOfferCount { get; set; }
    public double buyItNowPrice { get; set; }
    public string currencyType { get; set; } = "";
    public DateTime startDateTime { get; set; }
    public string startTime { get; set; } = "";
    public string itemId { get; set; } = "";
    public int quantity { get; set; }
    public int availableQuantity { get; set; }
    public string listingType { get; set; } = "";
    public string title { get; set; } = "";
    public string storageLocation { get; set; } = "";

    public int CompareTo(eBayListing? other)
    {
        if (other == null) return 1;

        return startDateTime.CompareTo(other.startDateTime);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        eBayListing listing = obj as eBayListing;
        if (listing == null)
        { 
            return false; 
        } 
        else 
        {
            return Equals(listing);
        }
    }

    public bool Equals(eBayListing? other)
    {
        return other?.itemId == itemId;
    }
}

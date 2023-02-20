using ebay_api_inventory.Entities;
using ebay_api_inventory.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Xml.Linq;

namespace ebay_api_inventory.Network;

public class MyEbaySelling
{
    public async Task<List<eBayListing>> Get(
        UserAccessToken accessToken, 
        int entriesPerPage, 
        int pageNumber)
    {
        eBaySystem ebaySystem = (eBaySystem)UserSettings.Default.System;
        string url = Url(ebaySystem);

        HttpRequestMessage request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri(url);
        request.Headers.Add("X-EBAY-API-CALL-NAME", "GetMyeBaySelling");
        request.Headers.Add("X-EBAY-API-SITEID", "0");
        request.Headers.Add("X-EBAY-API-COMPATIBILITY-LEVEL", "1097");
        request.Content = CreateBody(accessToken, entriesPerPage: entriesPerPage, pageNumber: pageNumber);

        var response = await App.requestClient.SendAsync(request);
        string responseString = await response.Content.ReadAsStringAsync();

        return ParseResponse(responseString);
    }

    private string Url(eBaySystem ebaySystem)
    {
        if (ebaySystem == eBaySystem.Sandbox)
        {
            return Endpoint.DevelopmentXML.Value;
        }

        return Endpoint.ProductionXML.Value;
    }
    
    private StringContent CreateBody(
        UserAccessToken accessToken, 
        int entriesPerPage, 
        int pageNumber)
    {
        string body = @"
        <?xml version=""1.0"" encoding=""UTF-8""?>
        <GetMyeBaySellingRequest xmlns=""urn:ebay:apis:eBLBaseComponents"">
            <RequesterCredentials>
                <eBayAuthToken>{0}</eBayAuthToken>
            </RequesterCredentials>
            <ActiveList>
                <Include>True</Include>
                <Pagination>
                    <EntriesPerPage>{1}</EntriesPerPage>
                    <PageNumber>{2}</PageNumber>
                </Pagination>
            </ActiveList>
        </GetMyeBaySellingRequest>";

        string bodyWithArgs = string.Format(
            body, 
            accessToken.access_token, 
            entriesPerPage, 
            pageNumber);

        return new StringContent(bodyWithArgs, Encoding.UTF8, "application/xml");
    }

    private List<eBayListing> ParseResponse(string xmlString)
    {
        List<eBayListing> listings = new List<eBayListing>();
        XDocument xmlDocument = XDocument.Parse(xmlString);
        var activeList = xmlDocument.Descendants().Where(p => p.Name.LocalName == "ActiveList");
        var activeItemArray = activeList.First().Descendants().Where(p => p.Name.LocalName == "ItemArray");
        var activeItems = activeItemArray.First().Descendants().Where(p => p.Name.LocalName == "Item");
        foreach(XElement item in activeItems)
        {
            eBayListing listing = new eBayListing();
            
            var bestOfferDetails = TryGetFirstNodeFrom(item, "BestOfferDetails");
            string bestOfferCount = TryGetValueOfFirstFrom(bestOfferDetails, "BestOfferCount");
            listing.bestOfferCount = int.Parse(bestOfferCount);

            var buyItNow = item.Descendants().First(p => p.Name.LocalName == "BuytItNowPrice");
            string buyItNowPrice = buyItNow.Value ?? "0.0";
            string buytItNowCurrency = buyItNow.Attribute("currencyID")?.Value ?? "";
            listing.buyItNowPrice = double.Parse(buyItNowPrice);
            listing.currencyType = buytItNowCurrency;

            string ebayNotes = item.Descendants().First(p => p.Name.LocalName == "eBayNotes").Value;
            listing.ebayNotes = ebayNotes;

            string itemId = item.Descendants().First(p => p.Name.LocalName == "ItemID").Value;
            listing.itemId = itemId;

            string quantity = item.Descendants().First(p => p.Name.LocalName == "Quantity").Value;
            listing.quantity = int.Parse(quantity);

            string availableQty = item.Descendants().First(p => p.Name.LocalName == "QuantityAvailable").Value;
            listing.availableQuantity = int.Parse(availableQty);

            string listingType = item.Descendants().First(p => p.Name.LocalName == "ListingType").Value;
            listing.listingType = listingType;

            string title = item.Descendants().First(p => p.Name.LocalName == "Title").Value;
            listing.title = title;

            listings.Add(listing);
        }

        return listings;
    }


    private XElement? TryGetFirstNodeFrom(XElement? element, string key)
    {
        try
        {
            return element?.Descendants().First(node => node.Name.LocalName == key);
        }
        catch { return null; }
    }

    private string TryGetValueOfFirstFrom(XElement? element, string key)
    {
        try
        {
            return TryGetFirstNodeFrom(element, key)?.Value ?? string.Empty;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return string.Empty;
        }
    }
}

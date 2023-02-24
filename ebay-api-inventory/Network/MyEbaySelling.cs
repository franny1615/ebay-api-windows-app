using ebay_api_inventory.Entities;
using ebay_api_inventory.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            int bestOfferCountInt;
            if (int.TryParse(bestOfferCount, NumberStyles.Number, CultureInfo.InvariantCulture, out bestOfferCountInt))
            {
                listing.bestOfferCount = bestOfferCountInt;
            }

            var buyItNow = TryGetFirstNodeFrom(item, "BuyItNowPrice");
            string buyItNowPrice = buyItNow?.Value ?? "0.0";
            double buyItNowPriceDouble;
            if (double.TryParse(buyItNowPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out buyItNowPriceDouble))
            {
                listing.buyItNowPrice = buyItNowPriceDouble;
            }

            string buytItNowCurrency = buyItNow?.Attribute("currencyID")?.Value ?? "";
            listing.currencyType = buytItNowCurrency;

            var ebayNotesNode = TryGetFirstNodeFrom(item, "eBayNotes");
            listing.ebayNotes = ebayNotesNode?.Value ?? "";

            var itemId = TryGetFirstNodeFrom(item, "ItemID");
            listing.itemId = itemId?.Value ?? "";

            var quantity =TryGetFirstNodeFrom(item, "Quantity");
            int qty;
            if (int.TryParse(quantity?.Value ?? "", NumberStyles.Number, CultureInfo.InvariantCulture, out qty)) 
            {
                listing.quantity = qty;
            }

            var availQuantity = TryGetFirstNodeFrom(item, "QuantityAvailable");
            int availQty;
            if (int.TryParse(availQuantity?.Value ?? "", NumberStyles.Number, CultureInfo.InvariantCulture, out availQty))
            {
                listing.availableQuantity = availQty;
            }

            var listingType = TryGetFirstNodeFrom(item, "ListingType");
            listing.listingType = listingType?.Value ?? "";

            var title = TryGetFirstNodeFrom(item, "Title");
            listing.title = title?.Value ?? "";

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

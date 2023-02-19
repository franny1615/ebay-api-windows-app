using ebay_api_inventory.Entities;
using ebay_api_inventory.Utilities;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ebay_api_inventory.Network;

public class MyEbaySelling
{
    public async Task<string> Get(
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
        ParseResponse(responseString);

        return responseString;
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

    private void ParseResponse(string xmlString)
    {
        XDocument xmlDocument = XDocument.Parse(xmlString);
        var activeListNodes = xmlDocument.Root?.Element("GetMyEbaySellingResponse")?.Element("ActiveList")?.Elements("ItemArray");

        if (activeListNodes != null)
        {
            foreach(var activeNode in activeListNodes)
            {
                Debug.WriteLine(activeNode.ToString());
            }
        }
    }
}

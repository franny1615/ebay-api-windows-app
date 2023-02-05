namespace ebay_api_inventory.Utilities;
public class Endpoint
{
    private Endpoint(string value) { Value = value; }
    
    public string Value { get; private set; }

    public static Endpoint ProductionXML { get { return new Endpoint("https://api.ebay.com/ws/api.dll"); } }
    public static Endpoint ProductionSOAP { get { return new Endpoint("https://api.ebay.com/wsapi"); } }
    public static Endpoint ProductionREST { get { return new Endpoint("https://api.ebay.com/"); } }
    public static Endpoint DevelopmentXML { get { return new Endpoint("https://api.sandbox.ebay.com/ws/api.dll"); } }
    public static Endpoint DevelopmentSOAP { get { return new Endpoint("https://api.sandbox.ebay.com/wsapi"); } }
    public static Endpoint DevelopmentREST { get { return new Endpoint("https://api.sandbox.ebay.com/"); } }
}

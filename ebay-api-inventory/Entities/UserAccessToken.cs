using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebay_api_inventory.Entities;

class UserAccessToken
{
    public string access_token { get; set; } = @"";
    public string expires_in { get; set; } = @"";
    public string refresh_token { get; set; } = @"";
    public string refresh_token_expires_in { get; set; } = @"";
    public string token_type { get;  set; } = @"";
}

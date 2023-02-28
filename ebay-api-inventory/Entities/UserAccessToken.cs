using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebay_api_inventory.Entities;

public class UserAccessToken
{
    public string access_token { get; set; } = @"";
    public int expires_in { get; set; } = 0;
    public string refresh_token { get; set; } = @"";
    public int refresh_token_expires_in { get; set; } = 0;
    public string token_type { get;  set; } = @"";
    public int insertedAtInSeconds { get; set; } = 0;
}

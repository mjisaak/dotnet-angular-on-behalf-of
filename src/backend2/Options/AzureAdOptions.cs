using System.Collections.Generic;

namespace backend.Options;

public class AzureAdOptions
{
    public string Instance { get; set; }
    public string Domain { get; set; }
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
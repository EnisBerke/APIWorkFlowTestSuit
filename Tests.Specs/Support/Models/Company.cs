using System.Text.Json.Serialization;

namespace MyApiAutomationProject.Tests.Specs.Support.Models;

public class CompanyRequest
{
    [JsonPropertyName("MainDomainValue")]
    public string MainDomainValue { get; set; }

    [JsonPropertyName("EcosystemId")]
    public int EcosystemId { get; set; }

    [JsonPropertyName("LicenseType")]
    public string LicenseType { get; set; } = "ContinuousAnnual";

    [JsonPropertyName("IsSubsidiary")]
    public bool IsSubsidiary { get; set; } = false;

    [JsonPropertyName("IsCloudProvider")]
    public bool IsCloudProvider { get; set; } = false;

    [JsonPropertyName("IgnoreDomainCheck")]
    public bool IgnoreDomainCheck { get; set; } = false;

    public CompanyRequest(string mainDomainValue, int ecosystemId)
    {
        MainDomainValue = mainDomainValue;
        EcosystemId = ecosystemId;
    }
}

using System.Text.Json.Serialization;

namespace MyApiAutomationProject.Tests.Specs.Support.Models;

public class FindingStatusUpdateRequest
{
    [JsonPropertyName("Status")]
    public string Status { get; set; }

    [JsonPropertyName("Comment")]
    public string Comment { get; set; } = "Approved to fix";

    public FindingStatusUpdateRequest(string status)
    {
        Status = status;
    }
}
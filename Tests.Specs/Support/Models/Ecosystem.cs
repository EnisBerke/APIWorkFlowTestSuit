using System.Text.Json.Serialization;

namespace MyApiAutomationProject.Tests.Specs.Support.Models;

public class EcosystemRequest
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    public EcosystemRequest(string name)
    {
        Name = name;
    }
}
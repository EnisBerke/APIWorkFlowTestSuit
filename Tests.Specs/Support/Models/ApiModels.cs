using System.Text.Json.Serialization;

namespace MyApiAutomationProject.Tests.Specs.Support.Models;

public class ApiSettings
{
    public string? BaseUrl { get; set; }
    public string? TokenUrl { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}
public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
} 
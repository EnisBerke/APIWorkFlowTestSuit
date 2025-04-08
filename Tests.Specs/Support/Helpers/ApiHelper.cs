using RestSharp;
using RestSharp.Authenticators;
using MyApiAutomationProject.Tests.Specs.Support.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MyApiAutomationProject.Tests.Specs.Support.Helpers;

public static class ApiHelper
{
    private static readonly ApiSettings _apiSettings;

    static ApiHelper()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        _apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>()
                    ?? throw new InvalidOperationException("ApiSettings not found in appsettings.json");

        Console.WriteLine($"[DEBUG] Loaded TokenUrl: {_apiSettings.TokenUrl}");
        Console.WriteLine($"[DEBUG] Loaded ClientId: {_apiSettings.ClientId}");
        Console.WriteLine($"[DEBUG] Loaded ClientSecret (Length): {_apiSettings.ClientSecret?.Length ?? 0}");

        if (string.IsNullOrWhiteSpace(_apiSettings.BaseUrl) ||
            string.IsNullOrWhiteSpace(_apiSettings.TokenUrl) ||
            string.IsNullOrWhiteSpace(_apiSettings.ClientId) ||
            string.IsNullOrWhiteSpace(_apiSettings.ClientSecret))
        {
            throw new InvalidOperationException("One or more required ApiSettings values are missing.");
        }
    }

    private static async Task<string> GetAccessTokenAsync()
    {
        var options = new RestClientOptions(_apiSettings.TokenUrl!)
        {
            Authenticator = new HttpBasicAuthenticator(_apiSettings.ClientId!, _apiSettings.ClientSecret!)
        };
        using var client = new RestClient(options);

        var request = new RestRequest("", Method.Post);
        request.AddParameter("grant_type", "client_credentials");

        var response = await client.ExecuteAsync<TokenResponse>(request);

        if (!response.IsSuccessful || response.Data?.AccessToken == null)
        {
            throw new InvalidOperationException($"Failed to get access token: {response.ErrorMessage} | {response.Content}");
        }

        return response.Data.AccessToken;
    }

    public static async Task<RestClient> CreateAuthenticatedClientAsync()
    {
        string accessToken = await GetAccessTokenAsync();

        var options = new RestClientOptions(_apiSettings.BaseUrl!)
        {
            Authenticator = new JwtAuthenticator(accessToken)
        };

        return new RestClient(options);
    }
} 
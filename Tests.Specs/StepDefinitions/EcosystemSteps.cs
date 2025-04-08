using RestSharp;
using TechTalk.SpecFlow;
using System.Threading.Tasks;
using MyApiAutomationProject.Tests.Specs.Support.Helpers;
using MyApiAutomationProject.Tests.Specs.Support.Models;
using FluentAssertions;
using MyApiAutomationProject.Tests.Specs.Support.Context;
using System;

namespace MyApiAutomationProject.Tests.Specs.StepDefinitions;

[Binding]
public sealed class EcosystemSteps
{
    private readonly ScenarioContext _scenarioContext;
    private RestClient? _apiClient;

    public EcosystemSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _scenarioContext.TryGetValue("ApiClient", out _apiClient);
    }

    [BeforeScenario(Order = 10)]
    public async Task InitializeApiClient()
    {
        _apiClient = await ApiHelper.CreateAuthenticatedClientAsync();
        _scenarioContext["ApiClient"] = _apiClient;
    }

    [When(@"I create ecosystem name as '(.*)'")]
    public async Task WhenICreateEcosystemNameAs(string ecosystemName)
    {
        if (_apiClient == null)
        {
            throw new InvalidOperationException("API client is not initialized.");
        }

        var requestBody = new EcosystemRequest(ecosystemName);
        var request = new RestRequest("/ecosystems", Method.Post);
        request.AddJsonBody(requestBody);

        var response = await _apiClient.ExecuteAsync(request);
        Console.WriteLine(response.Content ?? "<< Response Content is NULL >>");
        _scenarioContext["LastApiResponse"] = response;
    }

    [Then(@"the API response should be successful")]
    public void ThenTheApiResponseShouldBeSuccessful()
    {
        var response = _scenarioContext.Get<RestResponse>("LastApiResponse");
        response.IsSuccessful.Should().BeTrue($"API request failed. Status: {response.StatusCode}, Content: {response.Content}");
    }

    [When(@"Delete the ecosystem saved as '(.*)'")]
    public async Task WhenIDeleteTheEcosystemSavedAs(string ecosystemIdContextKey)
    {
        if (_apiClient == null)
            throw new InvalidOperationException("API client not found.");

        if (!SharedDataStore.TryGetValue<string>(ecosystemIdContextKey, out string? ecosystemId) || string.IsNullOrWhiteSpace(ecosystemId))
            throw new InvalidOperationException($"EcosystemId not found in SharedDataStore key '{ecosystemIdContextKey}'.");

        var request = new RestRequest($"/ecosystems/{ecosystemId}", Method.Delete);
        var response = await _apiClient.ExecuteAsync(request);
        _scenarioContext["LastApiResponse"] = response;
    }

    [Then(@"Get the details for the ecosystem saved as '(.*)'")]
    public async Task ThenGetTheDetailsForTheEcosystemSavedAs(string ecosystemIdContextKey)
    {
        if (_apiClient == null)
        {
            throw new InvalidOperationException("API client not found in ScenarioContext.");
        }

        if (!SharedDataStore.TryGetValue<string>(ecosystemIdContextKey, out string? ecosystemId) || string.IsNullOrWhiteSpace(ecosystemId))
        {
            throw new InvalidOperationException($"Valid EcosystemId not found in context key '{ecosystemIdContextKey}'.");
        }

        var request = new RestRequest($"/ecosystems/{ecosystemId}", Method.Get);
        var response = await _apiClient.ExecuteAsync(request);

        Console.WriteLine($"Ecosystem GET Response Body Start (ID: {ecosystemId})" + Environment.NewLine + (response.Content ?? "<< Response Content is NULL >>"));

        _scenarioContext["LastApiResponse"] = response;
    }
}
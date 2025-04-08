using RestSharp;
using TechTalk.SpecFlow;
using System.Threading.Tasks;
using MyApiAutomationProject.Tests.Specs.Support.Helpers;
using MyApiAutomationProject.Tests.Specs.Support.Models;
using FluentAssertions;
using System;
using MyApiAutomationProject.Tests.Specs.Support.Context;

namespace MyApiAutomationProject.Tests.Specs.StepDefinitions;

[Binding]
public sealed class CompanySteps
{
    private readonly ScenarioContext _scenarioContext;
    private RestClient? _apiClient;

    public CompanySteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _scenarioContext.TryGetValue("ApiClient", out _apiClient);
    }

    [When(@"I create a company with domain '(.*)' using the ecosystem ID saved as '(.*)'")]
    public async Task WhenICreateACompanyWithDomainUsingTheEcosystemIDSavedAs(string domainName, string ecosystemIdContextKey)
    {
        if (_apiClient == null)
        {
            throw new InvalidOperationException("API client not found in ScenarioContext.");
        }

        // SharedDataStore'dan oku
        if (!SharedDataStore.TryGetValue<string>(ecosystemIdContextKey, out string? ecosystemIdString) || 
            !int.TryParse(ecosystemIdString, out int ecosystemId))
        {
            throw new InvalidOperationException($"Valid integer EcosystemId not found in SharedDataStore key '{ecosystemIdContextKey}'. Actual stored value: '{ecosystemIdString ?? "NULL"}'");
        }

        var requestBody = new CompanyRequest(domainName, ecosystemId);
        var request = new RestRequest("/companies", Method.Post).AddJsonBody(requestBody);
        var response = await _apiClient.ExecuteAsync(request);
        Console.WriteLine(response.Content ?? "<< Response Content is NULL >>");
        _scenarioContext["LastApiResponse"] = response;
    }

    [Then(@"Get the details for the company saved as '(.*)'")]
    public async Task ThenGetTheDetailsForTheCompanySavedAs(string companyIdContextKey)
    {
        if (_apiClient == null)
        {
            throw new InvalidOperationException("API client not found in ScenarioContext.");
        }

        // SharedDataStore'dan oku
        if (!SharedDataStore.TryGetValue<string>(companyIdContextKey, out string? companyId) || string.IsNullOrWhiteSpace(companyId))
        {
            throw new InvalidOperationException($"CompanyId not found in SharedDataStore key '{companyIdContextKey}'.");
        }

        var request = new RestRequest($"/companies/{companyId}", Method.Get);
        var response = await _apiClient.ExecuteAsync(request);

        Console.WriteLine($"--- Company GET Response Body Start (ID: {companyId}) ---" + Environment.NewLine + (response.Content ?? "<< Response Content is NULL >>") + Environment.NewLine + $"--- Company GET Response Body End (ID: {companyId}) ---");

        _scenarioContext["LastApiResponse"] = response;
    }

    [When(@"Delete the company saved as '(.*)'")]
    public async Task WhenIDeleteTheCompanySavedAs(string companyIdContextKey)
    {
        if (_apiClient == null)
            throw new InvalidOperationException("API client not found.");

        // SharedDataStore'dan oku
        if (!SharedDataStore.TryGetValue<string>(companyIdContextKey, out string? companyId) || string.IsNullOrWhiteSpace(companyId))
            throw new InvalidOperationException($"CompanyId not found in SharedDataStore key '{companyIdContextKey}'.");

        var request = new RestRequest($"/companies/{companyId}", Method.Delete);
        var response = await _apiClient.ExecuteAsync(request);
        _scenarioContext["LastApiResponse"] = response;
    }
}
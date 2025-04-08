using RestSharp;
using TechTalk.SpecFlow;
using System.Threading.Tasks;
using MyApiAutomationProject.Tests.Specs.Support.Helpers;
using System;
using MyApiAutomationProject.Tests.Specs.Support.Models;
using MyApiAutomationProject.Tests.Specs.Support.Context;

namespace MyApiAutomationProject.Tests.Specs.StepDefinitions;

[Binding]
public sealed class FindingSteps
{
    private readonly ScenarioContext _scenarioContext;
    private RestClient? _apiClient;

    public FindingSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _scenarioContext.TryGetValue("ApiClient", out _apiClient);
    }

    [Then(@"Get email security findings for the company saved as '(.*)'")]
    public async Task ThenGetEmailSecurityFindingsForTheCompanySavedAs(string companyIdContextKey)
    {
        if (_apiClient == null)
        {
            throw new InvalidOperationException("API client not found.");
        }

        string? storedValue = null;
        bool keyExists = SharedDataStore.TryGetValue<string>(companyIdContextKey, out storedValue);
        Console.WriteLine($"[DEBUG] FindingSteps trying to read: Key='{companyIdContextKey}', Exists={keyExists}, Value='{storedValue ?? "NULL"}'");

        if (!keyExists || string.IsNullOrWhiteSpace(storedValue))
        {
            throw new InvalidOperationException($"Valid CompanyId not found in SharedDataStore key '{companyIdContextKey}'. Actual stored value: '{storedValue ?? "NULL"}'");
        }
        
        string companyId = storedValue;

        var request = new RestRequest($"/companies/{companyId}/findings/emailsecurity", Method.Get);
        request.AddParameter("page_number", 1);
        request.AddParameter("page_size", 10);

        var response = await _apiClient.ExecuteAsync(request);
        _scenarioContext["LastApiResponse"] = response;
    }

    [Then(@"Get the specific email security finding saved as '(.*)' for the company saved as '(.*)'")]
    public async Task ThenGetTheSpecificEmailSecurityFindingSavedAsForTheCompanySavedAs(string findingIdContextKey, string companyIdContextKey)
    {
        if (_apiClient == null)
        {
            throw new InvalidOperationException("API client not found.");
        }

        if (!SharedDataStore.TryGetValue<string>(companyIdContextKey, out string? companyId) || string.IsNullOrWhiteSpace(companyId))
        {
            throw new InvalidOperationException($"CompanyId not found in SharedDataStore key '{companyIdContextKey}'.");
        }
        
        if (!SharedDataStore.TryGetValue<string>(findingIdContextKey, out string? findingId) || string.IsNullOrWhiteSpace(findingId))
        {
            throw new InvalidOperationException($"FindingId not found in SharedDataStore key '{findingIdContextKey}'.");
        }

        var request = new RestRequest($"/companies/{companyId}/findings/emailsecurity/{findingId}", Method.Get);
        var response = await _apiClient.ExecuteAsync(request);
        _scenarioContext["LastApiResponse"] = response;
    }

    [When(@"Update the status to '(.*)' for the finding saved as '(.*)' for company saved as '(.*)'")]
    public async Task WhenIUpdateTheStatusToForTheFindingSavedAsForCompanySavedAs(string newStatus, string findingIdContextKey, string companyIdContextKey)
    {
        if (_apiClient == null)
        {
            throw new InvalidOperationException("API client not found.");
        }

        if (!SharedDataStore.TryGetValue<string>(companyIdContextKey, out string? companyId) || string.IsNullOrWhiteSpace(companyId))
        {
            throw new InvalidOperationException($"CompanyId not found in SharedDataStore key '{companyIdContextKey}'.");
        }
        
        if (!SharedDataStore.TryGetValue<string>(findingIdContextKey, out string? findingId) || string.IsNullOrWhiteSpace(findingId))
        {
            throw new InvalidOperationException($"FindingId not found in SharedDataStore key '{findingIdContextKey}'.");
        }

        var requestBody = new FindingStatusUpdateRequest(newStatus);
        
        var request = new RestRequest($"/companies/{companyId}/findings/{findingId}", Method.Patch);
        request.AddJsonBody(requestBody);
        var response = await _apiClient.ExecuteAsync(request);

        Console.WriteLine(response.Content ?? "<< NULL >>");
    }
}
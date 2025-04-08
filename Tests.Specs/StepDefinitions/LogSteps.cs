using RestSharp;
using TechTalk.SpecFlow;
using System.Threading.Tasks;
using MyApiAutomationProject.Tests.Specs.Support.Helpers;
using System;
using MyApiAutomationProject.Tests.Specs.Support.Context;

namespace MyApiAutomationProject.Tests.Specs.StepDefinitions;

[Binding]
public sealed class LogSteps
{
    private readonly ScenarioContext _scenarioContext;
    private RestClient? _apiClient;

    public LogSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _scenarioContext.TryGetValue("ApiClient", out _apiClient);
    }

    [Then(@"Get logs for the company saved as '(.*)' for date range '(.*)'")]
    public async Task ThenGetLogsForTheCompanySavedAsForDateRange(string companyIdContextKey, string dateRange)
    {
        if (_apiClient == null)
            throw new InvalidOperationException("API client not found.");

        if (!SharedDataStore.TryGetValue<string>(companyIdContextKey, out string? companyId) || string.IsNullOrWhiteSpace(companyId))
            throw new InvalidOperationException($"CompanyId not found in SharedDataStore key '{companyIdContextKey}'.");
        
        var request = new RestRequest("/log/company", Method.Get);
        request.AddParameter("id", companyId);
        request.AddParameter("page_number", 1);
        request.AddParameter("page_size", 10);
        request.AddParameter("date_range", dateRange);

        var response = await _apiClient.ExecuteAsync(request);
        _scenarioContext["LastApiResponse"] = response;
    }
} 
using RestSharp;
using TechTalk.SpecFlow;
using System.Threading.Tasks;
using MyApiAutomationProject.Tests.Specs.Support.Helpers;
using System;
using MyApiAutomationProject.Tests.Specs.Support.Context;

namespace MyApiAutomationProject.Tests.Specs.StepDefinitions;

[Binding]
public sealed class NotificationSteps
{
    private readonly ScenarioContext _scenarioContext;
    private RestClient? _apiClient;

    public NotificationSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _scenarioContext.TryGetValue("ApiClient", out _apiClient);
    }

    private async Task GetNotifications(string companyIdContextKey, string? status = null)
    {
        if (_apiClient == null)
            throw new InvalidOperationException("API client not found.");

        string? storedValue = null;
        bool keyExists = SharedDataStore.TryGetValue<string>(companyIdContextKey, out storedValue);
        Console.WriteLine($"[DEBUG] NotificationSteps trying to read: Key='{companyIdContextKey}', Exists={keyExists}, Value='{storedValue ?? "NULL"}'");

        if (!keyExists || string.IsNullOrWhiteSpace(storedValue))
        {
            throw new InvalidOperationException($"Valid CompanyId not found in SharedDataStore key '{companyIdContextKey}'. Actual stored value: '{storedValue ?? "NULL"}'"); // Hata mesajı güncellendi
        }
        string companyId = storedValue;

        var request = new RestRequest("/notifications", Method.Get);
        request.AddParameter("page_number", 1);
        request.AddParameter("page_size", 10);
        request.AddParameter("companyId", companyId);
        if (!string.IsNullOrEmpty(status))
        {
            request.AddParameter("status", status);
        }

        var response = await _apiClient.ExecuteAsync(request);
        _scenarioContext["LastApiResponse"] = response;
    }

    [Then(@"Get notifications for the company saved as '(.*)'")]
    public async Task ThenGetNotificationsForTheCompanySavedAs(string companyIdContextKey)
    {
        await GetNotifications(companyIdContextKey);
    }
}
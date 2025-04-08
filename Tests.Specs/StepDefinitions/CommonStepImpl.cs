using RestSharp;
using TechTalk.SpecFlow;
using System.Text.Json;
using FluentAssertions;
using System.Linq;
using MyApiAutomationProject.Tests.Specs.Support.Context;
using System.Collections.Generic;
using System;
using TechTalk.SpecFlow.Assist;
using System.Threading.Tasks;
using System.Threading;

namespace MyApiAutomationProject.Tests.Specs.StepDefinitions;

[Binding]
public sealed class CommonSteps
{
    private readonly ScenarioContext _scenarioContext;

    public CommonSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Then(@"Save response '(.*)' s value as '(.*)'")]
    public void ThenSaveResponseValueAs(string jsonKey, string contextKey)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("LastApiResponse not found or content is empty.");

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);
            if (!jsonDocument.RootElement.TryGetProperty(jsonKey, out JsonElement jsonElement))
                throw new KeyNotFoundException($"Key '{jsonKey}' not found in response.");

            string? value = jsonElement.ValueKind == JsonValueKind.Null ? null : jsonElement.ToString();
            SharedDataStore.Set(contextKey, value!);
            Console.WriteLine($"[DEBUG] Saved to SharedDataStore: Key='{contextKey}', Value='{value ?? "NULL"}'");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error validating key '{jsonKey}' in response: {response.Content}", ex);
        }
    }

    [Then(@"save random '(.*)' from root array where '(.*)' equals '(.*)' as '(.*)'")]
    public void ThenSaveRandomPropertyFromFilteredRootArrayAs(string propertyToSave, string filterProperty, string filterValue, string contextKey)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("Response unavailable or empty.");

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);
            if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidOperationException("Response root element is not a JSON array.");

            var filteredElements = jsonDocument.RootElement.EnumerateArray()
                .Where(el => el.ValueKind == JsonValueKind.Object &&
                            el.TryGetProperty(filterProperty, out JsonElement prop) &&
                            prop.ValueKind == JsonValueKind.String &&
                            prop.GetString()?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            if (filteredElements.Count == 0)
                throw new InvalidOperationException($"No elements found where '{filterProperty}' equals '{filterValue}'.");

            var random = new Random();
            int randomIndex = random.Next(filteredElements.Count);
            var randomElement = filteredElements[randomIndex];

            if (!randomElement.TryGetProperty(propertyToSave, out JsonElement propertyToSaveElement))
                throw new KeyNotFoundException($"Property '{propertyToSave}' not found in random element.");

            string? value = propertyToSaveElement.ValueKind == JsonValueKind.Null ? null : propertyToSaveElement.ToString();
            SharedDataStore.Set(contextKey, value!);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing filtered array. Filter: '{filterProperty}'='{filterValue}', Save: '{propertyToSave}'.", ex);
        }
    }

    [Then(@"save random index from root array where '(.*)' equals '(.*)' as '(.*)'")]
    public void ThenSaveRandomIndexFromFilteredRootArrayAs(string filterProperty, string filterValue, string contextKey)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("Response unavailable or empty.");

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);
            if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidOperationException("Response root element is not a JSON array.");

            var filteredElements = jsonDocument.RootElement.EnumerateArray()
                .Where(el => el.ValueKind == JsonValueKind.Object &&
                            el.TryGetProperty(filterProperty, out JsonElement prop) &&
                            prop.ValueKind == JsonValueKind.String &&
                            prop.GetString()?.Equals(filterValue, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            if (filteredElements.Count == 0)
                throw new InvalidOperationException($"No elements found where '{filterProperty}' equals '{filterValue}'.");

            var random = new Random();
            int randomIndex = random.Next(filteredElements.Count);
            SharedDataStore.Set(contextKey, randomIndex.ToString());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing filtered array to get random index. Filter: '{filterProperty}'='{filterValue}'.", ex);
        }
    }

    [Then(@"the response body key '(.*)' should have value '(.*)'")]
    public void ThenTheResponseBodyKeyShouldHaveValue(string jsonKey, string expectedValue)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("LastApiResponse not found or content is empty.");

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);

            if (!jsonDocument.RootElement.TryGetProperty(jsonKey, out JsonElement jsonElement))
                throw new KeyNotFoundException($"Key '{jsonKey}' not found in response.");

            string? actualValue = jsonElement.ValueKind == JsonValueKind.Null ? null : jsonElement.ToString();

            if (expectedValue.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                actualValue.Should().BeNull($"Expected key '{jsonKey}' to be null, but found '{actualValue}'.");
            }
            else
            {
                actualValue.Should().Be(expectedValue, $"Expected key '{jsonKey}' to be '{expectedValue}', but found '{actualValue}'.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error validating key '{jsonKey}' in response: {response.Content}", ex);
        }
    }

    [Then(@"save random '(.*)' from root array as '(.*)'")]
    public void ThenSaveRandomPropertyFromRootArrayAs(string propertyName, string contextKey)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("Response unavailable/empty.");

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);

            var arrayElements = jsonDocument.RootElement.EnumerateArray().ToList();

            var random = new Random();
            int randomIndex = random.Next(arrayElements.Count);
            var randomElement = arrayElements[randomIndex];

            if (!randomElement.TryGetProperty(propertyName, out JsonElement propertyElement))
                throw new KeyNotFoundException($"Property '{propertyName}' not found in random element.");

            string? value = propertyElement.ValueKind == JsonValueKind.Null ? null : propertyElement.ToString();
            SharedDataStore.Set(contextKey, value!);
            Console.WriteLine($"[DEBUG] Saved to SharedDataStore (Random Property): Key='{contextKey}', Value='{value ?? "NULL"}'");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting random property '{propertyName}'.", ex);
        }
    }

    [Then(@"the response body key '(.*)' should have one of the following values:")]
    public void ThenTheResponseBodyKeyShouldHaveOneOfValues(string jsonKey, Table possibleValuesTable)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("Response unavailable or empty.");

        var expectedOptions = possibleValuesTable.Rows
            .Select(row => row[0])
            .ToList();

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);
            var rootElement = jsonDocument.RootElement;

            if (!rootElement.TryGetProperty(jsonKey, out JsonElement jsonElement))
                throw new KeyNotFoundException($"Key '{jsonKey}' not found in response.");

            string? actualValue = jsonElement.ValueKind == JsonValueKind.Null ? null : jsonElement.ToString();

            bool matchFound = false;
            if (actualValue == null && expectedOptions.Contains("null", StringComparer.OrdinalIgnoreCase))
            {
                matchFound = true;
            }
            else if (actualValue != null)
            {
                var nonNullOptions = expectedOptions.Where(opt => !opt.Equals("null", StringComparison.OrdinalIgnoreCase));
                if (nonNullOptions.Contains(actualValue))
                {
                    matchFound = true;
                }
            }

            matchFound.Should().BeTrue(
                $"Key '{jsonKey}' should have one of the values [{string.Join(", ", expectedOptions)}], but found '{actualValue ?? "null"}'. Response: {response.Content}");

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error validating key '{jsonKey}' against possible values. Details: {ex.Message}", ex);
        }
    }

    [Then(@"the first element in the response array should have '(.*)' equal to '(.*)'")]
    public void ThenTheFirstElementInResponseArrayShouldHavePropertyEqualTo(string propertyName, string expectedValueLiteral)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("Response unavailable/empty.");

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);
            var firstElement = jsonDocument.RootElement.EnumerateArray().First();

            if (!firstElement.TryGetProperty(propertyName, out JsonElement propertyElement))
                throw new KeyNotFoundException($"Property '{propertyName}' not found.");

            string? actualValue = propertyElement.ValueKind == JsonValueKind.Null ? null : propertyElement.ToString();

            if (expectedValueLiteral.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                actualValue.Should().BeNull($"Property '{propertyName}' mismatch (null).");
            }
            else
            {
                actualValue.Should().Be(expectedValueLiteral, $"Property '{propertyName}' mismatch.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Validation failed for '{propertyName}' (literal).", ex);
        }
    }

    [Then(@"the first element in the response array should have '(.*)' equal to the value saved as '(.*)'")]
    public void ThenTheFirstElementInResponseArrayShouldHavePropertyEqualToContextValue(string propertyName, string contextKey)
    {
        if (!_scenarioContext.TryGetValue("LastApiResponse", out RestResponse response) || string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("Response unavailable/empty.");

        if (!SharedDataStore.TryGetValue<string>(contextKey, out string? expectedValue))
            throw new KeyNotFoundException($"Context key '{contextKey}' not found in SharedDataStore.");

        try
        {
            using var jsonDocument = JsonDocument.Parse(response.Content);
            var firstElement = jsonDocument.RootElement.EnumerateArray().First();

            if (!firstElement.TryGetProperty(propertyName, out JsonElement propertyElement))
                throw new KeyNotFoundException($"Property '{propertyName}' not found.");

            string? actualValue = propertyElement.ValueKind == JsonValueKind.Null ? null : propertyElement.ToString();

            if (expectedValue == null)
            {
                actualValue.Should().BeNull($"Property '{propertyName}' mismatch (context '{contextKey}', null).");
            }
            else
            {
                actualValue.Should().Be(expectedValue, $"Property '{propertyName}' mismatch (context '{contextKey}').");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Validation failed for '{propertyName}' (context '{contextKey}').", ex);
        }
    }

    [Then(@"Retry the last request until '(.*)' equals '(.*)' with max retries '(.*)' and delay '(.*)' seconds")]
    public async Task ThenIRetryTheLastRequestUntilKeyEqualsValue(string jsonKey, string expectedValue, int maxRetries, int delaySeconds)
    {
        if (!_scenarioContext.TryGetValue("LastApiClient", out RestClient client) ||
            !_scenarioContext.TryGetValue("LastApiRequest", out RestRequest request))
        {
            throw new InvalidOperationException("Last API Client or Request details not found in ScenarioContext.");
        }

        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            var response = await client.ExecuteAsync(request);
            _scenarioContext["LastApiResponse"] = response;

            if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
            {
                try
                {
                    using var jsonDocument = JsonDocument.Parse(response.Content);
                    if (jsonDocument.RootElement.TryGetProperty(jsonKey, out JsonElement jsonElement))
                    {
                        string? actualValue = jsonElement.ValueKind == JsonValueKind.Null ? null : jsonElement.ToString();
                        bool conditionMet = (expectedValue.Equals("null", StringComparison.OrdinalIgnoreCase) && actualValue == null) ||
                                            (actualValue != null && actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase));

                        if (conditionMet)
                        {
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error checking condition '{jsonKey}': {ex.Message}", ex);
                }
            }

            if (attempt < maxRetries)
            {
                await Task.Delay(delaySeconds * 1000);
            }
        }

        throw new TimeoutException($"Condition '{jsonKey}' == '{expectedValue}' was not met after {maxRetries} retries.");
    }
}

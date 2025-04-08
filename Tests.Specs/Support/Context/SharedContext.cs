using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyApiAutomationProject.Tests.Specs.Support.Context;

public static class SharedDataStore
{
    private static readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();

    public static void Set(string key, object value)
    {
        _values[key] = value;
    }

    public static T Get<T>(string key)
    {
        return (T)_values[key];
    }

    public static bool TryGetValue<T>(string key, out T? value)
    {
        if (_values.TryGetValue(key, out var objectValue) && objectValue is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    public static void Clear()
    {
        _values.Clear();
    }
}
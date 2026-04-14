using IO.FeatureHub.SSE.Model;

public async Task<bool> IsEnabled(string featureKey, string userKey = "simpleshop-user")
{
    try
    {
        if (_config == null)
        {
            Console.WriteLine("[FeatureHub] Config is null -> false");
            return false;
        }

        Console.WriteLine($"[FeatureHub] Checking feature '{featureKey}'...");

        var contextTask = _config.NewContext()
            .UserKey(userKey)
            .Country(StrategyAttributeCountryName.Denmark)
            .Build();

        var completed = await Task.WhenAny(contextTask, Task.Delay(2000));

        if (completed != contextTask)
        {
            Console.WriteLine("[FeatureHub] TIMEOUT -> returning false");
            return false;
        }

        var context = await contextTask;

        // 🔥 PRINT ALLE FEATURES
        Console.WriteLine("[FeatureHub] ALL FEATURES:");
        foreach (var key in context.Keys)
        {
            var value = context[key].Value;
            Console.WriteLine($"[FeatureHub] Feature key: {key} -> Value: {value}");
        }

        var rawValue = context[featureKey].Value;

        Console.WriteLine($"[FeatureHub] Feature: {featureKey}");
        Console.WriteLine($"[FeatureHub] Value: {rawValue}");
        Console.WriteLine($"[FeatureHub] Raw value type: {rawValue?.GetType()}");

        return rawValue is bool enabled && enabled;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[FeatureHub] ERROR: {ex}");
        return false;
    }
}
using FeatureHubSDK;
using IO.FeatureHub.SSE.Model;

namespace SimpleShop.Api.FeatureFlags;

public class FeatureStateProvider
{
    private readonly EdgeFeatureHubConfig _config;

    public FeatureStateProvider(IConfiguration configuration)
    {
        var edgeUrl = configuration["FeatureHub:EdgeUrl"]
                      ?? throw new InvalidOperationException("FeatureHub:EdgeUrl is missing.");

        var apiKey = configuration["FeatureHub:ApiKey"]
                     ?? throw new InvalidOperationException("FeatureHub:ApiKey is missing.");

        Console.WriteLine($"[FeatureHub] EdgeUrl: {edgeUrl}");
        Console.WriteLine($"[FeatureHub] ApiKey prefix: {apiKey[..Math.Min(8, apiKey.Length)]}...");

        _config = new EdgeFeatureHubConfig(edgeUrl, apiKey);
        _config.Init().Wait();

        Console.WriteLine("[FeatureHub] Init completed.");
    }

    public Task<bool> IsEnabled(string featureKey, string userKey = "test-user")
    {
        var context = _config.NewContext()
            .UserKey(userKey)
            .Country(StrategyAttributeCountryName.Denmark)
            .Build()
            .GetAwaiter()
            .GetResult();

        var value = context[featureKey].Value;

        Console.WriteLine($"[FeatureHub] Feature: {featureKey}");
        Console.WriteLine($"[FeatureHub] Value: {value}");
        Console.WriteLine($"[FeatureHub] Type: {value?.GetType()}");

        if (value is bool boolValue)
            return Task.FromResult(boolValue);

        return Task.FromResult(false);
    }
}
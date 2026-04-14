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

        var isEnabled = context.IsEnabled(featureKey);
        var isSet = context.IsSet(featureKey);

        Console.WriteLine($"[FeatureHub] Feature: {featureKey}");
        Console.WriteLine($"[FeatureHub] IsEnabled: {isEnabled}");
        Console.WriteLine($"[FeatureHub] IsSet: {isSet}");

        return Task.FromResult(isSet && isEnabled);
    }

    public Task<object> GetDebugInfo(string featureKey, string userKey = "test-user")
    {
        var context = _config.NewContext()
            .UserKey(userKey)
            .Country(StrategyAttributeCountryName.Denmark)
            .Build()
            .GetAwaiter()
            .GetResult();

        var isEnabled = context.IsEnabled(featureKey);
        var isSet = context.IsSet(featureKey);
        var rawValue = context[featureKey].Value;
        var stringValue = context[featureKey].StringValue;

        Console.WriteLine($"[FeatureHub] Feature: {featureKey}");
        Console.WriteLine($"[FeatureHub] IsEnabled: {isEnabled}");
        Console.WriteLine($"[FeatureHub] IsSet: {isSet}");
        Console.WriteLine($"[FeatureHub] RawValue: {rawValue}");
        Console.WriteLine($"[FeatureHub] StringValue: {stringValue}");

        return Task.FromResult<object>(new
        {
            featureKey,
            isEnabled,
            isSet,
            rawValue,
            stringValue
        });
    }
}
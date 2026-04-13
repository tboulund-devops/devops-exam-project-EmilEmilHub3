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

        var config = new EdgeFeatureHubConfig(edgeUrl, apiKey);

        config.Init().Wait();
        _config = config;
    }

    public bool IsEnabled(string featureKey, string userKey = "simpleshop-user")
    {
        var context = _config.NewContext()
            .UserKey(userKey)
            .Country(StrategyAttributeCountryName.Denmark)
            .Build()
            .GetAwaiter()
            .GetResult();

        var rawValue = context[featureKey].Value;

        Console.WriteLine($"[FeatureHub] Feature '{featureKey}' raw value: {rawValue} (type: {rawValue?.GetType().Name ?? "null"})");

        return rawValue is bool enabled && enabled;
    }
}
using FeatureHubSDK;
using IO.FeatureHub.SSE.Model;

namespace SimpleShop.Api.FeatureFlags;

public class FeatureStateProvider
{
    private readonly EdgeFeatureHubConfig? _config;

    public FeatureStateProvider(IConfiguration configuration)
    {
        try
        {
            var edgeUrl = configuration["FeatureHub:EdgeUrl"]
                          ?? throw new InvalidOperationException("FeatureHub:EdgeUrl is missing.");

            var apiKey = configuration["FeatureHub:ApiKey"]
                         ?? throw new InvalidOperationException("FeatureHub:ApiKey is missing.");

            Console.WriteLine($"[FeatureHub] EdgeUrl: {edgeUrl}");
            Console.WriteLine($"[FeatureHub] ApiKey prefix: {apiKey[..Math.Min(8, apiKey.Length)]}...");
            Console.WriteLine("[FeatureHub] Creating EdgeFeatureHubConfig...");

            var config = new EdgeFeatureHubConfig(edgeUrl, apiKey);

            Console.WriteLine("[FeatureHub] Calling Init()...");
            config.Init().Wait();
            Console.WriteLine("[FeatureHub] Init() completed.");

            _config = config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FeatureHub] Init failed: {ex}");
        }
    }

    public bool IsEnabled(string featureKey, string userKey = "simpleshop-user")
    {
        try
        {
            Console.WriteLine($"[FeatureHub] IsEnabled called for feature '{featureKey}' and user '{userKey}'.");

            if (_config == null)
            {
                Console.WriteLine("[FeatureHub] _config is null. Returning false.");
                return false;
            }

            Console.WriteLine("[FeatureHub] Building context...");
            var context = _config.NewContext()
                .UserKey(userKey)
                .Country(StrategyAttributeCountryName.Denmark)
                .Build()
                .GetAwaiter()
                .GetResult();

            Console.WriteLine("[FeatureHub] Context built.");

            var featureState = context[featureKey];
            Console.WriteLine($"[FeatureHub] Feature state object: {featureState}");

            var rawValue = featureState.Value;
            Console.WriteLine($"[FeatureHub] Feature '{featureKey}' raw value: {rawValue} (type: {rawValue?.GetType().Name ?? "null"})");

            return rawValue is bool enabled && enabled;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FeatureHub] Failed to evaluate '{featureKey}': {ex}");
            return false;
        }
    }
}
using FeatureHubSDK;
using IO.FeatureHub.SSE.Model;

namespace SimpleShop.Api.FeatureFlags;

public class FeatureStateProvider
{
    private readonly EdgeFeatureHubConfig? _config;
    private readonly Task? _initTask;

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

            _config = new EdgeFeatureHubConfig(edgeUrl, apiKey);
            _initTask = _config.Init();

            Console.WriteLine("[FeatureHub] Init started.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FeatureHub] Init setup failed: {ex}");
        }
    }

    public async Task<bool> IsEnabled(string featureKey, string userKey = "simpleshop-user")
    {
        try
        {
            if (_config == null || _initTask == null)
            {
                Console.WriteLine("[FeatureHub] Config/init task is null -> false");
                return false;
            }

            await _initTask;

            Console.WriteLine($"[FeatureHub] Checking feature '{featureKey}' for user '{userKey}'");

            var context = await _config.NewContext()
                .UserKey(userKey)
                .Country(StrategyAttributeCountryName.Denmark)
                .Build();

            var featureState = context[featureKey];
            var rawValue = featureState?.Value;

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
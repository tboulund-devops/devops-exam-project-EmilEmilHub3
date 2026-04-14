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
            Console.WriteLine($"[FeatureHub] Init failed: {ex}");
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

            var context = await _config.NewContext()
                .UserKey(userKey)
                .Country(StrategyAttributeCountryName.Denmark)
                .Build();

            var featureState = context[featureKey];

            if (featureState == null)
            {
                Console.WriteLine($"[FeatureHub] FeatureState for '{featureKey}' is null");
                return false;
            }

            Console.WriteLine($"[FeatureHub] FeatureState object for '{featureKey}': {featureState}");

            var value = featureState.Value;

            if (value == null)
            {
                Console.WriteLine($"[FeatureHub] Feature '{featureKey}' returned null value");
                return false;
            }

            Console.WriteLine($"[FeatureHub] Feature: {featureKey}");
            Console.WriteLine($"[FeatureHub] Value: {value}");
            Console.WriteLine($"[FeatureHub] Type: {value.GetType()}");

            if (value is bool boolValue)
                return boolValue;

            if (value is string strValue)
                return strValue.Equals("true", StringComparison.OrdinalIgnoreCase);

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FeatureHub] ERROR: {ex}");
            return false;
        }
    }
}
}
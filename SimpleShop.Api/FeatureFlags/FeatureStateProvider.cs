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

            var config = new EdgeFeatureHubConfig(edgeUrl, apiKey);

            Task.Run(async () =>
            {
                Console.WriteLine("[FeatureHub] Init async...");
                await config.Init();
                Console.WriteLine("[FeatureHub] Init done.");
            });

            _config = config;
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

            // Debug konkrete features
            var productSearchValue = context["ProductSearch"].Value;
            var productDeleteValue = context["ProductDelete"].Value;

            Console.WriteLine($"[FeatureHub] ProductSearch value: {productSearchValue}");
            Console.WriteLine($"[FeatureHub] ProductSearch type: {productSearchValue?.GetType()}");
            Console.WriteLine($"[FeatureHub] ProductDelete value: {productDeleteValue}");
            Console.WriteLine($"[FeatureHub] ProductDelete type: {productDeleteValue?.GetType()}");

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
}
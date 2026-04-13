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

        var value = context[featureKey].Value;

        return value is bool enabled && enabled;
    }
}
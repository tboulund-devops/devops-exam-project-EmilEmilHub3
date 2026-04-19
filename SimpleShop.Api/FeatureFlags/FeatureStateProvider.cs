using FeatureHubSDK;
using IO.FeatureHub.SSE.Model;

namespace SimpleShop.Api.FeatureFlags;

/// <summary>
/// Provides access to FeatureHub feature toggle states.
/// Responsible for initializing the FeatureHub Edge client,
/// evaluating feature flags, and exposing debug information.
/// </summary>
public class FeatureStateProvider
{
    private readonly EdgeFeatureHubConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStateProvider"/> class.
    /// Reads required FeatureHub configuration values and starts the client connection.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration containing FeatureHub settings.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required FeatureHub configuration values are missing.
    /// </exception>
    public FeatureStateProvider(IConfiguration configuration)
    {
        var edgeUrl = configuration["FeatureHub:EdgeUrl"]
                      ?? throw new InvalidOperationException("FeatureHub:EdgeUrl is missing.");

        var apiKey = configuration["FeatureHub:ApiKey"]
                     ?? throw new InvalidOperationException("FeatureHub:ApiKey is missing.");

        // Startup diagnostics used to verify injected configuration values.
        Console.WriteLine($"[FeatureHub] EdgeUrl: {edgeUrl}");
        Console.WriteLine($"[FeatureHub] ApiKey prefix: {apiKey[..Math.Min(8, apiKey.Length)]}...");

        // Create and initialize FeatureHub Edge configuration.
        _config = new EdgeFeatureHubConfig(edgeUrl, apiKey);
        _config.Init().Wait();

        Console.WriteLine("[FeatureHub] Init completed.");
    }

    /// <summary>
    /// Determines whether a feature toggle is enabled
    /// for the specified user context.
    /// </summary>
    /// <param name="featureKey">
    /// The unique feature toggle key.
    /// </param>
    /// <param name="userKey">
    /// Optional user identifier used for targeting rules.
    /// Default value is test-user.
    /// </param>
    /// <returns>
    /// <c>true</c> if the feature is both configured and enabled;
    /// otherwise <c>false</c>.
    /// </returns>
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

        // Runtime diagnostics for feature evaluation.
        Console.WriteLine($"[FeatureHub] Feature: {featureKey}");
        Console.WriteLine($"[FeatureHub] IsEnabled: {isEnabled}");
        Console.WriteLine($"[FeatureHub] IsSet: {isSet}");

        return Task.FromResult(isSet && isEnabled);
    }

    /// <summary>
    /// Returns extended debug information for a feature toggle.
    /// Useful for frontend diagnostics, testing, and troubleshooting.
    /// </summary>
    /// <param name="featureKey">
    /// The unique feature toggle key.
    /// </param>
    /// <param name="userKey">
    /// Optional user identifier used for targeting rules.
    /// Default value is test-user.
    /// </param>
    /// <returns>
    /// An object containing the evaluated feature state.
    /// </returns>
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

        // Runtime diagnostics for detailed feature inspection.
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
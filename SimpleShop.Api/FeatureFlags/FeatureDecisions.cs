namespace SimpleShop.Api.FeatureFlags;

/// <summary>
/// Centralizes business decisions based on feature toggles.
/// This class isolates feature flag logic from controllers
/// and services to keep the application easier to maintain.
/// </summary>
public class FeatureDecisions
{
    private readonly FeatureStateProvider _featureStateProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureDecisions"/> class.
    /// </summary>
    /// <param name="featureStateProvider">
    /// Service used to retrieve feature toggle states.
    /// </param>
    public FeatureDecisions(FeatureStateProvider featureStateProvider)
    {
        _featureStateProvider = featureStateProvider;
    }

    /// <summary>
    /// Determines whether product search functionality is enabled.
    /// </summary>
    /// <returns>
    /// <c>true</c> if product search is enabled; otherwise <c>false</c>.
    /// </returns>
    public async Task<bool> CanSearchProducts()
    {
        return await _featureStateProvider.IsEnabled("ProductSearch");
    }

    /// <summary>
    /// Determines whether product deletion functionality is enabled.
    /// </summary>
    /// <returns>
    /// <c>true</c> if product deletion is enabled; otherwise <c>false</c>.
    /// </returns>
    public async Task<bool> CanDeleteProducts()
    {
        return await _featureStateProvider.IsEnabled("ProductDelete");
    }
}
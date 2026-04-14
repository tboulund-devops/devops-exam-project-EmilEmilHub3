namespace SimpleShop.Api.FeatureFlags;

public class FeatureDecisions
{
    private readonly FeatureStateProvider _featureStateProvider;

    public FeatureDecisions(FeatureStateProvider featureStateProvider)
    {
        _featureStateProvider = featureStateProvider;
    }

    public async Task<bool> CanSearchProducts()
    {
        return await _featureStateProvider.IsEnabledAsync("ProductSearch");
    }

    public async Task<bool> CanDeleteProducts()
    {
        return await _featureStateProvider.IsEnabledAsync("ProductDelete");
    }
}
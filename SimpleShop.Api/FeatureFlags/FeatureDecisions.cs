namespace SimpleShop.Api.FeatureFlags;

public class FeatureDecisions
{
    private readonly FeatureStateProvider _featureStateProvider;

    public FeatureDecisions(FeatureStateProvider featureStateProvider)
    {
        _featureStateProvider = featureStateProvider;
    }

    public bool CanSearchProducts()
    {
        return _featureStateProvider.IsEnabled("ProductSearch");
    }

    public bool CanDeleteProducts()
    {
        return _featureStateProvider.IsEnabled("ProductDelete");
    }
}
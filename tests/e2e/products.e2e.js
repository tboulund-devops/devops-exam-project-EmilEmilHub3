import { Selector } from 'testcafe';

/**
 * Base URL for the application under test.
 * 
 * In CI/CD pipelines, BASE_URL is typically injected as an environment variable
 * (e.g., pointing to staging or production).
 * 
 * When running locally, it falls back to localhost.
 */
const baseUrl = process.env.BASE_URL || 'http://127.0.0.1:8080';

/**
 * Test suite for the SimpleShop product UI.
 * 
 * All tests in this fixture will start from the defined base URL.
 */
fixture`SimpleShop products UI`
    .page`${baseUrl}`;

/**
 * E2E Test: User can create a product.
 * 
 * Purpose:
 * - Verifies that the product creation flow works end-to-end.
 * - Ensures the UI, API, and database integration function correctly.
 * 
 * Steps:
 * 1. Enter product name and price
 * 2. Click create button
 * 3. Verify success message
 * 4. Verify product appears in the table
 */
test('user can create a product', async t => {
    // Generate a unique product name to avoid collisions between test runs
    const uniqueName = `E2E Product ${Date.now()}`;

    // Select UI elements using data-testid attributes (stable selectors for testing)
    const productNameInput = Selector('[data-testid="product-name-input"]');
    const productPriceInput = Selector('[data-testid="product-price-input"]');
    const createProductButton = Selector('[data-testid="create-product-button"]');
    const statusMessage = Selector('[data-testid="status-message"]');
    const matchingCell = Selector('td').withText(uniqueName);

    await t
        // Fill in product form
        .typeText(productNameInput, uniqueName)
        .typeText(productPriceInput, '123.45')

        // Submit product creation
        .click(createProductButton)

        // Verify success feedback is shown to the user
        .expect(statusMessage.innerText).contains('Created product')

        // Verify the created product appears in the UI table
        .expect(matchingCell.exists).ok();
});

/**
 * E2E Test: User can search for a product when the search feature is enabled.
 * 
 * Purpose:
 * - Verifies conditional UI behavior controlled by feature flags.
 * - Ensures search functionality works when enabled.
 * - Ensures search UI is hidden/disabled when feature is off.
 * 
 * Behavior:
 * - If search feature is disabled → test verifies UI is not present (PASS)
 * - If search feature is enabled → test executes full search flow
 */
test('user can search for a product when search feature is enabled', async t => {
    const uniqueName = `E2E Search Product ${Date.now()}`;

    // Select core UI elements
    const productNameInput = Selector('[data-testid="product-name-input"]');
    const productPriceInput = Selector('[data-testid="product-price-input"]');
    const createProductButton = Selector('[data-testid="create-product-button"]');

    // Select search-related UI elements (feature-flag controlled)
    const searchInput = Selector('[data-testid="search-input"]');
    const searchButton = Selector('[data-testid="search-button"]');

    const statusMessage = Selector('[data-testid="status-message"]');
    const matchingCell = Selector('td').withText(uniqueName);

    /**
     * Determine whether the search feature is visible in the UI.
     * 
     * This indirectly reflects the state of the feature toggle (e.g., FeatureHub).
     */
    const searchFeatureVisible = await searchInput.exists && await searchButton.exists;

    // First, create a product (shared setup step for search test)
    await t
        .typeText(productNameInput, uniqueName)
        .typeText(productPriceInput, '123.45')
        .click(createProductButton)
        .expect(statusMessage.innerText).contains('Created product');

    /**
     * If the search feature is disabled:
     * - Verify that the search UI is not visible
     * - Exit test early (no failure)
     */
    if (!searchFeatureVisible) {
        await t.expect(searchInput.exists).notOk('Search feature is disabled in this environment.');
        return;
    }

    /**
     * If the search feature is enabled:
     * - Perform search
     * - Validate that the created product is found
     */
    await t
        .typeText(searchInput, uniqueName, { replace: true })
        .click(searchButton)

        // Verify search result feedback
        .expect(statusMessage.innerText).contains('Loaded 1 product')

        // Verify the product appears in the filtered results
        .expect(matchingCell.exists).ok();
});
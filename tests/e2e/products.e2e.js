import { Selector } from 'testcafe';

/**
 * Base URL for the application under test.
 *
 * In CI/CD pipelines, BASE_URL is typically injected as an environment variable
 * (for example, pointing to staging).
 *
 * When running locally, it falls back to localhost.
 */
const baseUrl = process.env.BASE_URL || 'http://127.0.0.1:8080';

/**
 * Test suite for the SimpleShop product UI.
 *
 * All tests in this fixture start from the configured base URL.
 */
fixture`SimpleShop products UI`
    .page`${baseUrl}`;

/**
 * E2E Test: User can create a product.
 *
 * Purpose:
 * - Verifies that the product creation flow works end-to-end.
 * - Confirms that the UI can submit data and display the created product.
 */
test('user can create a product', async t => {
    const uniqueName = `E2E Product ${Date.now()}`;

    const productNameInput = Selector('[data-testid="product-name-input"]');
    const productPriceInput = Selector('[data-testid="product-price-input"]');
    const createProductButton = Selector('[data-testid="create-product-button"]');
    const statusMessage = Selector('[data-testid="status-message"]');
    const matchingCell = Selector('td').withText(uniqueName);

    await t
        // Fill in the create form
        .typeText(productNameInput, uniqueName)
        .typeText(productPriceInput, '123.45')

        // Submit the product
        .click(createProductButton)

        // Verify success feedback
        .expect(statusMessage.innerText).contains('Created product')

        // Verify the created product is shown in the table
        .expect(matchingCell.exists).ok();
});

/**
 * E2E Test: User can search for a product when the search feature is enabled.
 *
 * Purpose:
 * - Verifies conditional UI behavior controlled by feature flags.
 * - Confirms that search works when the feature is enabled.
 * - Confirms that the search UI stays hidden when the feature is disabled.
 *
 * Important:
 * - A selector can exist in the DOM even when the element is hidden.
 * - Therefore, this test checks visibility instead of only existence.
 */
test('user can search for a product when search feature is enabled', async t => {
    const uniqueName = `E2E Search Product ${Date.now()}`;

    const productNameInput = Selector('[data-testid="product-name-input"]');
    const productPriceInput = Selector('[data-testid="product-price-input"]');
    const createProductButton = Selector('[data-testid="create-product-button"]');

    const searchInput = Selector('[data-testid="search-input"]');
    const searchButton = Selector('[data-testid="search-button"]');

    const statusMessage = Selector('[data-testid="status-message"]');
    const matchingCell = Selector('td').withText(uniqueName);

    // Create a product first so there is something to search for
    await t
        .typeText(productNameInput, uniqueName)
        .typeText(productPriceInput, '123.45')
        .click(createProductButton)
        .expect(statusMessage.innerText).contains('Created product');

    /**
     * Determine whether the search feature is actually usable.
     *
     * Using .exists is not enough here, because the search controls may still
     * be present in the DOM while being hidden with CSS or disabled.
     */
    const searchFeatureVisible = await searchInput.visible && await searchButton.visible;

    /**
     * If the search feature is disabled in the current environment,
     * verify that the search UI is not visible and end the test successfully.
     */
    if (!searchFeatureVisible) {
        await t
            .expect(searchInput.visible).notOk('Search input should not be visible when the feature is disabled.')
            .expect(searchButton.visible).notOk('Search button should not be visible when the feature is disabled.');
        return;
    }

    /**
     * If the search feature is enabled,
     * perform the search and verify the expected result.
     */
    await t
        .typeText(searchInput, uniqueName, { replace: true })
        .click(searchButton)

        // Verify search result feedback
        .expect(statusMessage.innerText).contains('Loaded 1 product')

        // Verify the product appears in the filtered results
        .expect(matchingCell.exists).ok();
});
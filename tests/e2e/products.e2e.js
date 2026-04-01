import { Selector } from 'testcafe';

const baseUrl = process.env.BASE_URL || 'http://127.0.0.1:8080';

fixture`SimpleShop products UI`
    .page`${baseUrl}`;

test('user can create and search for a product', async t => {
    const uniqueName = `E2E Product ${Date.now()}`;
    const productNameInput = Selector('[data-testid="product-name-input"]');
    const productPriceInput = Selector('[data-testid="product-price-input"]');
    const createProductButton = Selector('[data-testid="create-product-button"]');
    const searchInput = Selector('[data-testid="search-input"]');
    const searchButton = Selector('[data-testid="search-button"]');
    const statusMessage = Selector('[data-testid="status-message"]');
    const matchingCell = Selector('td').withText(uniqueName);

    await t
        .typeText(productNameInput, uniqueName)
        .typeText(productPriceInput, '123.45')
        .click(createProductButton)
        .expect(statusMessage.innerText).contains('Created product')
        .typeText(searchInput, uniqueName, { replace: true })
        .click(searchButton)
        .expect(statusMessage.innerText).contains('Loaded 1 product')
        .expect(matchingCell.exists).ok();
});
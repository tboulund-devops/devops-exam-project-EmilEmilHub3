// Cache frequently used DOM elements to avoid repeated lookups.
const productsTableBody = document.getElementById('products-table-body');
const statusMessage = document.getElementById('status-message');
const createProductForm = document.getElementById('create-product-form');
const productNameInput = document.getElementById('product-name');
const productPriceInput = document.getElementById('product-price');
const searchInput = document.getElementById('search-input');
const searchButton = document.getElementById('search-button');
const loadAllButton = document.getElementById('load-all-button');

// The search section is shown or hidden depending on the feature toggle state.
const searchSection = searchInput.closest('.card');

// Store the current client-side feature state.
const featureState = {
    productSearch: false,
    productDelete: false
};

/**
 * Displays a status message to the user.
 * @param {string} message - The message to display.
 * @param {boolean} [isError=false] - Determines whether the message is shown as an error.
 */
function setStatus(message, isError = false) {
    statusMessage.textContent = message;
    statusMessage.className = isError ? 'error' : 'success';
}

/**
 * Updates the search UI based on the current feature toggle state.
 * Hides the section and disables input when product search is not enabled.
 */
function updateSearchUI() {
    if (searchSection) {
        searchSection.style.display = featureState.productSearch ? '' : 'none';
    }

    searchInput.disabled = !featureState.productSearch;
    searchButton.disabled = !featureState.productSearch;

    if (!featureState.productSearch) {
        searchInput.value = '';
    }
}

/**
 * Loads the state of a single feature toggle from the API.
 * @param {string} featureName - The feature key to evaluate.
 * @returns {Promise<boolean>} True if the feature is enabled; otherwise false.
 */
async function isFeatureEnabled(featureName) {
    const response = await fetch(`/api/feature-toggles/${featureName}`);

    if (!response.ok) {
        throw new Error(`Could not load feature toggle: ${featureName}`);
    }

    const result = await response.json();
    return result.isEnabled;
}

/**
 * Loads all feature toggles required by the frontend.
 * Updates the UI after the toggle state has been resolved.
 */
async function loadFeatureToggles() {
    try {
        featureState.productSearch = await isFeatureEnabled('ProductSearch');
        featureState.productDelete = await isFeatureEnabled('ProductDelete');

        updateSearchUI();
    } catch (error) {
        setStatus('Could not load feature toggles.', true);
    }
}

/**
 * Creates the delete button for a product row.
 * The button is disabled if the delete feature is turned off.
 * @param {number} productId - The identifier of the product to delete.
 * @returns {HTMLButtonElement} A configured delete button.
 */
function createDeleteButton(productId) {
    const button = document.createElement('button');
    button.textContent = 'Delete';
    button.setAttribute('data-testid', `delete-product-${productId}`);

    if (!featureState.productDelete) {
        button.disabled = true;
        button.title = 'Delete feature is disabled';
        return button;
    }

    button.addEventListener('click', async () => {
        try {
            const response = await fetch(`/api/products/${productId}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                const errorBody = await response.json().catch(() => null);
                const errorMessage = errorBody?.error ?? 'Could not delete product.';
                throw new Error(errorMessage);
            }

            setStatus(`Deleted product ${productId}`);
            await loadProducts(searchInput.value.trim());
        } catch (error) {
            setStatus(error.message, true);
        }
    });

    return button;
}

/**
 * Renders the product list into the products table.
 * @param {Array} products - The products returned from the API.
 */
function renderProducts(products) {
    productsTableBody.innerHTML = '';

    for (const product of products) {
        const row = document.createElement('tr');
        row.setAttribute('data-testid', `product-row-${product.id}`);

        row.innerHTML = `
            <td>${product.id}</td>
            <td class="table-row-name" data-testid="product-name-${product.id}">${product.name}</td>
            <td data-testid="product-price-${product.id}">${Number(product.price).toFixed(2)}</td>
            <td><div class="table-actions"></div></td>
        `;

        row.querySelector('.table-actions').appendChild(createDeleteButton(product.id));
        productsTableBody.appendChild(row);
    }
}

/**
 * Loads products from the API.
 * If a search term is provided, the list is filtered by product name.
 * @param {string} [search=''] - Optional product name search term.
 */
async function loadProducts(search = '') {
    try {
        if (search && !featureState.productSearch) {
            throw new Error('Search feature is disabled.');
        }

        const query = search ? `?search=${encodeURIComponent(search)}` : '';
        const response = await fetch(`/api/products${query}`);

        if (!response.ok) {
            const errorBody = await response.json().catch(() => null);
            const errorMessage = errorBody?.error ?? 'Could not load products.';
            throw new Error(errorMessage);
        }

        const products = await response.json();
        renderProducts(products);

        if (search) {
            setStatus(`Loaded ${products.length} product(s) matching "${search}"`);
        } else {
            setStatus(`Loaded ${products.length} product(s)`);
        }
    } catch (error) {
        setStatus(error.message, true);
    }
}

// Handle product creation form submission.
createProductForm.addEventListener('submit', async (event) => {
    event.preventDefault();

    const payload = {
        name: productNameInput.value,
        price: Number(productPriceInput.value)
    };

    try {
        const response = await fetch('/api/products', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            const errorBody = await response.json().catch(() => null);
            const errorMessage = errorBody?.error ?? 'Could not create product.';
            throw new Error(errorMessage);
        }

        productNameInput.value = '';
        productPriceInput.value = '';
        setStatus(`Created product ${payload.name}`);
        await loadProducts(searchInput.value.trim());
    } catch (error) {
        setStatus(error.message, true);
    }
});

// Handle search button clicks.
searchButton.addEventListener('click', async () => {
    if (!featureState.productSearch) {
        setStatus('Search feature is disabled.', true);
        return;
    }

    await loadProducts(searchInput.value.trim());
});

// Handle load all button clicks and reset the current search.
loadAllButton.addEventListener('click', async () => {
    searchInput.value = '';
    await loadProducts();
});

/**
 * Initializes the frontend by loading feature toggles first,
 * then loading the initial product list.
 */
async function init() {
    await loadFeatureToggles();
    await loadProducts();
}

init();
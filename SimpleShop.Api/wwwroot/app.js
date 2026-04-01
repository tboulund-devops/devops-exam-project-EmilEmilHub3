const productsTableBody = document.getElementById('products-table-body');
const statusMessage = document.getElementById('status-message');
const createProductForm = document.getElementById('create-product-form');
const productNameInput = document.getElementById('product-name');
const productPriceInput = document.getElementById('product-price');
const searchInput = document.getElementById('search-input');
const searchButton = document.getElementById('search-button');
const loadAllButton = document.getElementById('load-all-button');

function setStatus(message, isError = false) {
    statusMessage.textContent = message;
    statusMessage.className = isError ? 'error' : 'success';
}

function createDeleteButton(productId) {
    const button = document.createElement('button');
    button.textContent = 'Delete';
    button.setAttribute('data-testid', `delete-product-${productId}`);

    button.addEventListener('click', async () => {
        try {
            const response = await fetch(`/api/products/${productId}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error('Could not delete product.');
            }

            setStatus(`Deleted product ${productId}`);
            await loadProducts(searchInput.value.trim());
        } catch (error) {
            setStatus(error.message, true);
        }
    });

    return button;
}

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

async function loadProducts(search = '') {
    try {
        const query = search ? `?search=${encodeURIComponent(search)}` : '';
        const response = await fetch(`/api/products${query}`);

        if (!response.ok) {
            throw new Error('Could not load products.');
        }

        const products = await response.json();
        renderProducts(products);
        setStatus(`Loaded ${products.length} product(s)`);
    } catch (error) {
        setStatus(error.message, true);
    }
}

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

searchButton.addEventListener('click', async () => {
    await loadProducts(searchInput.value.trim());
});

loadAllButton.addEventListener('click', async () => {
    searchInput.value = '';
    await loadProducts();
});

loadProducts();
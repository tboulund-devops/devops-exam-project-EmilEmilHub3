using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Api.Services;

/// <summary>
/// Provides business logic related to shopping cart operations.
/// Responsible for validation, cart item creation, retrieval,
/// updates, and removal.
/// </summary>
public class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartService"/> class.
    /// </summary>
    /// <param name="cartRepository">
    /// Repository used for cart item persistence.
    /// </param>
    /// <param name="userRepository">
    /// Repository used to validate that users exist.
    /// </param>
    /// <param name="productRepository">
    /// Repository used to validate that products exist.
    /// </param>
    public CartService(
        ICartRepository cartRepository,
        IUserRepository userRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Adds a product to a user's cart after validating
    /// the user, product, and quantity.
    /// </summary>
    /// <param name="dto">
    /// The cart item data to add.
    /// </param>
    /// <returns>
    /// A response DTO representing the created cart item.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when required values are missing,
    /// invalid, or refer to non-existing entities.
    /// </exception>
    public async Task<CartItemResponseDto> AddItemAsync(AddCartItemDto dto)
    {
        if (!dto.UserId.HasValue || dto.UserId.Value <= 0)
            throw new ArgumentException("UserId must be greater than 0.", nameof(dto));

        if (!dto.ProductId.HasValue || dto.ProductId.Value <= 0)
            throw new ArgumentException("ProductId must be greater than 0.", nameof(dto));

        if (!dto.Quantity.HasValue || dto.Quantity.Value <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(dto));

        var userId = dto.UserId.Value;
        var productId = dto.ProductId.Value;
        var quantity = dto.Quantity.Value;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new ArgumentException("User was not found.", nameof(dto));

        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
            throw new ArgumentException("Product was not found.", nameof(dto));

        var cartItem = new CartItem
        {
            UserId = userId,
            ProductId = productId,
            Quantity = quantity
        };

        var created = await _cartRepository.AddAsync(cartItem);

        // Fallback to the already loaded product if the repository did not hydrate it.
        var createdProduct = created.Product ?? product;

        return new CartItemResponseDto
        {
            Id = created.Id,
            ProductId = createdProduct.Id,
            ProductName = createdProduct.Name,
            Price = createdProduct.Price,
            Quantity = created.Quantity
        };
    }

    /// <summary>
    /// Gets the current cart for a specific user.
    /// </summary>
    /// <param name="userId">
    /// The identifier of the user whose cart should be returned.
    /// </param>
    /// <returns>
    /// A response DTO containing cart items and total price.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the user identifier is invalid
    /// or the user does not exist.
    /// </exception>
    public async Task<CartResponseDto> GetCartAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than 0.", nameof(userId));

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new ArgumentException("User was not found.", nameof(userId));

        var cartItems = await _cartRepository.GetByUserIdAsync(userId);

        var items = cartItems.Select(item => new CartItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? string.Empty,
            Price = item.Product?.Price ?? 0,
            Quantity = item.Quantity
        }).ToList();

        return new CartResponseDto
        {
            UserId = userId,
            Items = items,
            TotalPrice = items.Sum(i => i.Price * i.Quantity)
        };
    }

    /// <summary>
    /// Updates the quantity of an existing cart item.
    /// </summary>
    /// <param name="id">
    /// The identifier of the cart item.
    /// </param>
    /// <param name="dto">
    /// The new quantity data.
    /// </param>
    /// <returns>
    /// A response DTO representing the updated cart item.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the identifier or quantity is invalid.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the cart item does not exist.
    /// </exception>
    public async Task<CartItemResponseDto> UpdateItemQuantityAsync(int id, UpdateCartItemQuantityDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("Cart item id must be greater than 0.", nameof(id));

        if (!dto.Quantity.HasValue || dto.Quantity.Value <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(dto));

        var cartItem = await _cartRepository.GetByIdAsync(id);
        if (cartItem is null)
            throw new KeyNotFoundException("Cart item was not found.");

        cartItem.Quantity = dto.Quantity.Value;

        var updated = await _cartRepository.UpdateAsync(cartItem);

        return new CartItemResponseDto
        {
            Id = updated.Id,
            ProductId = updated.ProductId,
            ProductName = updated.Product?.Name ?? string.Empty,
            Price = updated.Product?.Price ?? 0,
            Quantity = updated.Quantity
        };
    }

    /// <summary>
    /// Removes a cart item from the cart.
    /// </summary>
    /// <param name="id">
    /// The identifier of the cart item to remove.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous remove operation.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the identifier is invalid.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the cart item does not exist.
    /// </exception>
    public async Task RemoveItemAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Cart item id must be greater than 0.", nameof(id));

        var cartItem = await _cartRepository.GetByIdAsync(id);
        if (cartItem is null)
            throw new KeyNotFoundException("Cart item was not found.");

        await _cartRepository.DeleteAsync(cartItem);
    }
}
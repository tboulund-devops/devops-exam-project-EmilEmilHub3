using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Api.Services;

public class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IUserRepository userRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

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
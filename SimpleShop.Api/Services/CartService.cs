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
        if (dto.UserId <= 0)
            throw new ArgumentException("UserId must be greater than 0.", nameof(dto));

        if (dto.ProductId <= 0)
            throw new ArgumentException("ProductId must be greater than 0.", nameof(dto));

        if (dto.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(dto));

        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null)
            throw new ArgumentException("User was not found.", nameof(dto));

        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product is null)
            throw new ArgumentException("Product was not found.", nameof(dto));

        var cartItem = new CartItem
        {
            UserId = dto.UserId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
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
}
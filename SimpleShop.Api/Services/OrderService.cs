using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Api.Services;

/// <summary>
/// Provides business logic for order creation.
/// Responsible for validating the user, reading cart contents,
/// creating an order, and clearing the cart afterwards.
/// </summary>
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderService"/> class.
    /// </summary>
    /// <param name="orderRepository">
    /// Repository used for order persistence.
    /// </param>
    /// <param name="cartRepository">
    /// Repository used to read and clear cart items.
    /// </param>
    /// <param name="userRepository">
    /// Repository used to validate that the user exists.
    /// </param>
    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Creates a new order from the current contents of a user's cart.
    /// The cart is cleared after the order has been created successfully.
    /// </summary>
    /// <param name="dto">
    /// The order creation request data.
    /// </param>
    /// <returns>
    /// A response DTO containing the created order and calculated totals.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the user identifier is invalid,
    /// the user does not exist, or the cart is empty.
    /// </exception>
    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        if (!dto.UserId.HasValue || dto.UserId.Value <= 0)
            throw new ArgumentException("UserId must be greater than 0.", nameof(dto));

        var userId = dto.UserId.Value;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new ArgumentException("User was not found.", nameof(dto));

        var cartItems = await _cartRepository.GetByUserIdAsync(userId);
        if (cartItems.Count == 0)
            throw new ArgumentException("Cart is empty.", nameof(dto));

        var order = new Order
        {
            UserId = userId,
            OrderLines = cartItems.Select(item => new OrderLine
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product?.Price ?? 0m
            }).ToList()
        };

        var created = await _orderRepository.AddAsync(order);

        // Clear the cart after the order has been saved successfully.
        await _cartRepository.ClearByUserIdAsync(userId);

        return new OrderResponseDto
        {
            Id = created.Id,
            UserId = created.UserId,
            OrderLines = created.OrderLines.Select(line => new OrderLineResponseDto
            {
                ProductId = line.ProductId,
                ProductName = line.Product?.Name ?? string.Empty,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LineTotal = line.UnitPrice * line.Quantity
            }).ToList(),
            TotalPrice = created.OrderLines.Sum(line => line.UnitPrice * line.Quantity)
        };
    }
}
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Api.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _userRepository = userRepository;
    }

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
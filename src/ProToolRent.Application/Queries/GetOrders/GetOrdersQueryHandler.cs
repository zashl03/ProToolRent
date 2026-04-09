using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Enums;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<List<OrderSummaryDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IToolRepository _toolRepository;

    public GetOrdersQueryHandler(IUserRepository userRepository, IOrderRepository orderRepository, IToolRepository toolRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _toolRepository = toolRepository;
    }

    public async Task<Result<List<OrderSummaryDto>>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if(user == null)
            return Result<List<OrderSummaryDto>>.NotFound("User not found");
        
        if(user.Role == UserRole.Tenant)
            return await GetTenantOrderSummary(user, ct);

        if(user.Role == UserRole.Landlord)
            return await GetLandlordOrderSummary(user, ct);
        
        return Result<List<OrderSummaryDto>>.Failure("Admin cant check orders");
    }

    public async Task<Result<List<OrderSummaryDto>>> GetTenantOrderSummary(User tenant, CancellationToken ct)
    {
        List<Order> orders = await _orderRepository.GetOrdersByTenantAsync(tenant.Id, ct);

        var orderSummary = new List<OrderSummaryDto>(orders.Count);
        if(orders.Count == 0)    
            return Result<List<OrderSummaryDto>>.Success(orderSummary); 

        foreach(var order in orders)
        {
            var orderItem = order.OrderItems.FirstOrDefault();
            if (orderItem is null)
                return Result<List<OrderSummaryDto>>.Failure("Order item not found");
                
            var tool = orderItem.Tool;
            var landlord = await _userRepository.GetByIdAsync(tool.UserId, ct);
            if(landlord == null)
                return Result<List<OrderSummaryDto>>.NotFound("Landlord not found");
            
            var itemSummary = new OrderSummaryDto(
                order.Id,
                tool.Id,
                tool.Specification.Name,
                tool.Specification.Brand,
                tool.ImageUrl,
                tool.Price,
                orderItem.Quantity,
                orderItem.CreatedDate,
                orderItem.EndDate,
                orderItem.Cost,
                order.Status,
                order.CreatedDate,
                tenant.Profile.FirstName + " " + tenant.Profile.LastName,   
                landlord.Profile.FirstName + " " + landlord.Profile.LastName 
            );

            orderSummary.Add(itemSummary);
        }

        return Result<List<OrderSummaryDto>>.Success(orderSummary);
    }

    public async Task<Result<List<OrderSummaryDto>>> GetLandlordOrderSummary(User landlord, CancellationToken ct)
    {
        List<Order> orders = await _orderRepository.GetOrdersByLandlordAsync(landlord.Id, ct);
        var orderSummary = new List<OrderSummaryDto>(orders.Count);

        if(orders.Count == 0)    
            return Result<List<OrderSummaryDto>>.Success(orderSummary); 

        foreach(var order in orders)
        {
            var orderItem = order.OrderItems.FirstOrDefault();
            if (orderItem is null)
                return Result<List<OrderSummaryDto>>.Failure("Order item not found");

            var tool = orderItem.Tool;

            var tenant = await _userRepository.GetByIdAsync(order.UserId, ct);
            if(tenant == null)
                return Result<List<OrderSummaryDto>>.NotFound("Tenant not found");
            
            var itemSummary = new OrderSummaryDto(
                order.Id,
                tool.Id,
                tool.Specification.Name,
                tool.Specification.Brand,
                tool.ImageUrl,
                tool.Price,
                orderItem.Quantity,
                orderItem.CreatedDate,
                orderItem.EndDate,
                orderItem.Cost,
                order.Status,
                order.CreatedDate,
                tenant.Profile.FirstName + " " + tenant.Profile.LastName,   
                landlord.Profile.FirstName + " " + landlord.Profile.LastName 
            );

            orderSummary.Add(itemSummary);
        }

        return Result<List<OrderSummaryDto>>.Success(orderSummary);
    }
}
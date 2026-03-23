using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, ct);

        if(order == null)
        {
            return Result<OrderDto>.NotFound($"Order with {request.Id} not found");
        }
        var orderDto = new OrderDto(
            order.Id,
            order.Status,
            order.CreatedDate,
            order.EndDate,
            order.OrderItems.Select(x => new OrderItemDto(
                x.Id, 
                x.CreatedDate, 
                x.EndDate, 
                x.Cost, 
                x.Quantity, 
                x.ToolId
                )).ToList()
            );
        return Result<OrderDto>.Success(orderDto);
    }
}

using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;

namespace ProToolRent.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderSummaryDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IToolRepository _toolRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IToolRepository toolRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _toolRepository = toolRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderSummaryDto>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var order = new Order(request.UserId);
        var tool = await _toolRepository.GetByIdAsync(request.ToolId, ct);

        if(tool == null)
            return Result<OrderSummaryDto>.NotFound($"Tool with {request.ToolId} not found");
        if(request.Quantity > tool.Quantity.Available)
            return Result<OrderSummaryDto>.Conflict("Available quantity less than requested");

        order.AddItem(request.StartDate, request.EndDate, request.Quantity, tool);

        await _orderRepository.AddAsync(order, ct);

        await _unitOfWork.SaveChangeAsync(ct);

        var item = order.OrderItems.First();
        var tenant = await _userRepository.GetByIdAsync(order.UserId, ct);
        var landlord = await _userRepository.GetByIdAsync(tool.UserId, ct);

        if(tenant == null)
            return Result<OrderSummaryDto>.Conflict("Tenant not found");
        if(landlord == null)
            return Result<OrderSummaryDto>.Conflict("Landlord not found");

        var orderSummaryDto = new OrderSummaryDto(
            order.Id,
            tool.Id,
            tool.Specification.Name,
            tool.Specification.Brand,
            tool.ImageUrl,
            tool.Price,
            item.Quantity,
            item.CreatedDate,
            item.EndDate,
            item.Cost,
            order.Status,
            order.CreatedDate,
            tenant.Profile.FirstName + " " + tenant.Profile.LastName,
            landlord.Profile.FirstName + " " + landlord.Profile.LastName
        );

        return Result<OrderSummaryDto>.Success(orderSummaryDto);
    }
}

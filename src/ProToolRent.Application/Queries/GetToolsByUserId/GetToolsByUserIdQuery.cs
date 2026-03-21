using MediatR;
using ProToolRent.Application.Common;
using ProToolRent.Application.DTOs;

namespace ProToolRent.Application.Queries.GetToolsByUserId;

public record GetToolsByUserIdQuery(Guid Id) : IRequest<Result<List<ToolDto>>>;

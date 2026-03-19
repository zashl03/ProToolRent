using MediatR;
using ProToolRent.Application.Common;
namespace ProToolRent.Application.Commands.DeleteTool;

public record DeleteToolCommand(Guid Id) : IRequest<Result>;
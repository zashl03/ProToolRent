using MediatR;
using ProToolRent.Application.Common;

public record UploadToolImageCommand
(
    Guid ToolId,
    string ImageUrl
) : IRequest<Result>;
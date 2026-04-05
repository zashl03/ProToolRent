
public record UploadImageRequest
(
    Guid ToolId,
    IFormFile File
);
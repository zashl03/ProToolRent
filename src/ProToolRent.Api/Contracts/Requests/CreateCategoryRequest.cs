namespace ProToolRent.Api.Contracts.Requests;

public record CreateCategoryRequest
(
    string Name,
    Guid? ParentId
);


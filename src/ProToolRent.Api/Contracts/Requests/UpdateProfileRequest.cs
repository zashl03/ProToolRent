

public record UpdateProfileRequest
(
    Guid UserId,
    string FirstName,
    string LastName,
    string City,
    string Organization,
    string Phone
);
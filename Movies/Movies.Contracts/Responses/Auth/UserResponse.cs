namespace Movies.Contracts.Responses.Auth;

public sealed record UserResponse(Guid Id, string Name, string Email, IList<string> Roles);
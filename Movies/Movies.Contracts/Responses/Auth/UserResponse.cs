namespace Movies.Contracts.Responses.Auth;

public sealed record UserResponse(string Name, string Email, IList<string> Roles);
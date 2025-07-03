namespace Movies.Contracts.Requests.Auth;

public record ConfirmEmailRequest
{
    public required string UserId { get; init; }
    
    public required string Token { get; init; }
}
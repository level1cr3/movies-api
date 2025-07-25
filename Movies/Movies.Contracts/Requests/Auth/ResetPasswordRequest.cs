namespace Movies.Contracts.Requests.Auth;

public record ResetPasswordRequest(string UserId, string Token, string NewPassword);
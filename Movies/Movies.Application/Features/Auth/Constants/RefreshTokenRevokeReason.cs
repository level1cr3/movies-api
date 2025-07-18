namespace Movies.Application.Features.Auth.Constants;

public static class RefreshTokenRevokeReason
{
    public const string UserLoggedOut = "UserLoggedOut";
    
    public const string TokenRotated  = "RefreshTokenRotated";
    
    public const string ReuseDetected = "RefreshTokenReuseDetected";
    
    public const string AdminRevoked = "AdminRevoked";
}
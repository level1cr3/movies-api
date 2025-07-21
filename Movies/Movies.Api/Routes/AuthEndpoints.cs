namespace Movies.Api.Routes;

public static class AuthEndpoints
{
    private const string Base = $"{ApiEndpointConstants.ApiBase}/auth";

    public const string Register = $"{Base}/register";

    public const string ConfirmEmail = $"{Base}/confirm-email";

    public const string Login = $"{Base}/login";
    
    public const string RefreshToken = $"{Base}/refresh-token";
    
    public const string Logout = $"{Base}/logout";

    
    // TODO
    // resend confirmation email.
    // forgot password.
    // reset password.
    
    
    // I will allow multiple device logins. so I won't revoke all the tokens during login or logout.
    // only remove the one that user have but how will i know which one the user have ?
    // well I will send the refresh token for logout and revoke It. hence successful logout. else if token is already
    // revoked return bad request. 
    
    
    // TODO later
    // maybe later 2factor auth. using TOTP.
    // OAuth or external login.
}


/*
 
 - since register,login,logout and refresh etc are not resource they are action they can be put into like api/auth/register like endpoints.
 - we don't have to follow standard restapi structure here which is one resource per controller like api/movies and let methods decide the operations on them.
 
 */
using Microsoft.Extensions.Options;
using Movies.Api.Routes;
using Movies.Api.Settings;

namespace Movies.Api.Services;

public interface IRefreshTokenCookieService
{
    string? GetRefreshToken();

    void SendRefreshToken(string token, DateTimeOffset expiresAt);

    void DeleteRefreshToken();
}

public class RefreshTokenCookieService(IHttpContextAccessor httpContextAccessor, IOptions<RefreshTokenCookieSettings> options)
    : IRefreshTokenCookieService
{
    private readonly RefreshTokenCookieSettings _refreshTokenCookieSettings = options.Value;
    private const string RefreshTokenCookieName = "refreshToken";

    public string? GetRefreshToken()
    {
        httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken);
        return refreshToken;
    }

    public void SendRefreshToken(string token, DateTimeOffset expiresAt)
    {
        var cookieOptions = CreateCookieOptions(expiresAt);
        httpContextAccessor.HttpContext!.Response.Cookies.Append(RefreshTokenCookieName, token, cookieOptions);
    }

    public void DeleteRefreshToken()
    {
        var cookieOptions = CreateCookieOptions(DateTimeOffset.UtcNow.AddDays(-1));
        httpContextAccessor.HttpContext!.Response.Cookies.Delete(RefreshTokenCookieName, cookieOptions);
    }

    private CookieOptions CreateCookieOptions(DateTimeOffset expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true, // 👈 makes cookie not accessible by JS
            Secure = _refreshTokenCookieSettings.Secure,
            SameSite = Enum.Parse<SameSiteMode>(_refreshTokenCookieSettings.SameSite),
            // set none if frontend & backend are on different domains. if same domain then strict
            Expires = expiresAt,
            Path = _refreshTokenCookieSettings.Path, // 👈 restrict to /api/auth because we want refresh cookie in login, logout, refresh-token
        };
        
        // refresh token in JSON is only for non-browser clients. For browsers (mobile or desktop), always use HttpOnly cookie.
    }
}

/*
 Notes on Domain option of cookie response and same-site option as well. 
 
   Since in production your React app and API are served from the **same domain** (even if Kestrel runs locally and Apache proxies requests), you **don’t need to set `Domain` at all**.
   
   * **Local dev:** omit `Domain`, browser will scope cookie to `localhost` automatically.
   * **Production:** omit `Domain`, browser will scope cookie to `example.com`.
   
   Your cookie will work for all requests **to the same origin** (same scheme + host + port). Just make sure:
   
   * `Secure = true` in production (HTTPS).
   * `HttpOnly = true` for security.
   * `SameSite = Lax` or `Strict` unless you need cross-site requests.
   
   This keeps your code simpler and avoids headaches with domain scoping.
   
   You don’t need to ever set `Domain` unless you want **cross-subdomain sharing**, e.g., `api.example.com` → `app.example.com`.
   
   
   
   // Domain = ".example.com" // 👈 if using subdomains like app.example.com + api.example.com
   // It controls which hostnames can receive the cookie. if domains are different don't set domain
 
 
 
 Yes — you can safely set `SameSite = Strict` **in your case**, because:
   
   * Your React app and API are on the **same origin** (same scheme + host + port in production via Apache reverse proxy).
   * `Strict` only blocks cookies from being sent in **cross-site requests**, which you won’t have if the frontend and API share the same origin.
   
   ---
   
   ### Quick summary for your setup:
   
   | Environment       | Domain | SameSite | Notes                                                                                       |
   | ----------------- | ------ | -------- | ------------------------------------------------------------------------------------------- |
   | Local development | omit   | Strict   | Works because dev app talks to `localhost` directly. Use `credentials: "include"` in fetch. |
   | Production        | omit   | Strict   | Works because API and app are served under same domain (`example.com`).                     |
   
   ---
   
   ⚠️ **Only change SameSite** if in the future you ever serve the API on a **different subdomain** from the frontend and need the browser to send cookies cross-site. Then you’d need:
   
   ```csharp
   SameSite = SameSiteMode.None
   Secure = true
   ```
   
   ---
   
   If you want, I can write a **production-ready `CreateCookieOptions` snippet** for you that handles **dev vs prod**, sets Secure correctly, and avoids Domain entirely. It’s very handy.
   
 
 
 
 */
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Movies.Application.Shared.Foundation;

internal interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
}

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;
    
    public string? UserId => _user?.FindFirstValue(ClaimTypes.NameIdentifier);
    
    public string? Email => _user?.FindFirstValue(ClaimTypes.Email);
    
    public string? UserName => _user?.Identity?.Name;
    
    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<string> Roles => _user?.FindAll(ClaimTypes.Role).Select(r => r.Value) ?? [];
}

/*
 
    public Guid? UserId =>
        _user?.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId ? Guid.Parse(userId) : null;
    
    // is { } userId → checks “is not null” and, if true, assigns it to userId.

 
 public Guid? UserId =>
   _user?.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId && Guid.TryParse(userId, out var guid)
       ? guid
       : null;
       
Use TryParse only if you expect external tokens/claims that you don’t fully control 
(like an external IdP or JWT issued by a third party).
 
 */
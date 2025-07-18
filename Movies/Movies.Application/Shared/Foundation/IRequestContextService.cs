using Microsoft.AspNetCore.Http;

namespace Movies.Application.Shared.Foundation;

internal interface IRequestContextService
{
    string GetClientIp();
}

internal class RequestContextService(IHttpContextAccessor httpContextAccessor) : IRequestContextService
{
    public string GetClientIp()
    {
        return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }
}
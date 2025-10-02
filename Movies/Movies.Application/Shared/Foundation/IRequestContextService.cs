using Microsoft.AspNetCore.Http;

namespace Movies.Application.Shared.Foundation;

internal interface IRequestContextService
{
    string ClientIp { get; }
}

internal class RequestContextService(IHttpContextAccessor httpContextAccessor) : IRequestContextService
{
    public string ClientIp => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
}
using Microsoft.AspNetCore.Http;

namespace Movies.Application.Shared.Services;

internal interface IRequestContextService
{
    string ClientIp { get; }
}

internal class RequestContextService(IHttpContextAccessor httpContextAccessor) : IRequestContextService
{
    public string ClientIp => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
}
using Movies.Application.Data.Entities;

namespace Movies.Application.Data.Repositories.JwtRefreshToken;

internal interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    void Create(RefreshToken refreshToken);

    Task RevokeAsync(Guid id, string revokeReason, string revokeByIp, Guid? replaceByTokenId = null,
        CancellationToken cancellationToken = default);
}
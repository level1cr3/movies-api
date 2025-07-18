using Microsoft.EntityFrameworkCore;
using Movies.Application.Data.Entities;

namespace Movies.Application.Data.Repositories.JwtRefreshToken;

internal class RefreshTokenRepository(ApplicationDbContext db) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await db.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId, cancellationToken: cancellationToken);
    }

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken: cancellationToken);
    }

    public void Create(RefreshToken refreshToken)
    {
        db.RefreshTokens.Add(refreshToken);
    }
    
    public async Task Revoke(Guid id, string revokeReason, string revokeByIp, Guid? replaceByTokenId = null, CancellationToken cancellationToken = default)
    {
        var refresh = await db.RefreshTokens.FindAsync([id], cancellationToken: cancellationToken);
        
        if (refresh is not null)
        {
            refresh.IsRevoked = true;
            refresh.RevokedReason = revokeReason;
            refresh.RevokedAt = DateTime.UtcNow;
            refresh.RevokedByIp = revokeByIp;
            refresh.ReplacedByTokenId = replaceByTokenId;
        }
    }

}
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
        return await db.RefreshTokens.Where(rt => rt.Token == refreshToken)
            .Include(rt => rt.User).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

    public void Create(RefreshToken refreshToken)
    {
        db.RefreshTokens.Add(refreshToken);
        // don't add db.savechanges() here. read below as to why ?
    }
    
    public async Task RevokeAsync(Guid id, string revokeReason, string revokeByIp, Guid? replaceByTokenId = null, CancellationToken cancellationToken = default)
    {
        var refresh = await db.RefreshTokens.FindAsync([id], cancellationToken: cancellationToken);
        
        if (refresh is not null)
        {
            refresh.IsRevoked = true;
            refresh.RevokedReason = revokeReason;
            refresh.RevokedAt = DateTime.UtcNow;
            refresh.RevokedByIp = revokeByIp;
            refresh.ReplacedByTokenId ??= replaceByTokenId;
            
            // NOTE: SaveChangesAsync is called here directly to ensure immediate persistence of security-related actions.
            // Other repositories defer SaveChangesAsync to the service layer for batching and transaction consistency.
            await db.SaveChangesAsync(cancellationToken);
        }
    }

}


/*
 Notes: 
 It’s common to break the “no SaveChanges in repo” rule for things like refresh tokens, password resets, logins, etc.
 These are often handled as special cases due to their security, transactional isolation, and framework-driven workflows.
 
 Why?
   Auth workflows are atomic and isolated. You rarely batch refresh token revokes with other changes.
   Consistency with the rest of your codebase is good, but not a hard rule for auth.
   Security: You want to persist changes immediately (revokes, logouts, etc.), not risk leaving them unsaved.
   
   
   
# Should you call db.SaveChangesAsync() (or db.SaveChanges()) inside Create?
   Enterprise Patterns & Pragmatism
   Repository Pattern Best Practice:
   Generally, repositories should not call SaveChangesAsync() themselves. Instead, they make changes (e.g., Add, Update, Remove), and the service layer (or a Unit of Work) calls SaveChangesAsync() once, after all necessary changes are staged.
   This allows for batching, transactional consistency, and easier testing.
   
   Why?
   
   Atomicity: You might want to create several entities in one transaction.
   Consistency: Keeps your pattern uniform—repositories stage changes, services commit them.
   Flexibility: Lets you roll back or aggregate changes before saving.
  
   Auth/Refresh Token Exception?
   In security/auth scenarios, sometimes you want immediate persistence (like for RevokeAsync), as discussed earlier.
   But for Create (adding a new refresh token), it's usually not as critical to persist instantly, unless you have a specific security reason.
   Pragmatic, Real-World Recommendation
   Keep Create as is:
   Let it stage the addition (db.RefreshTokens.Add(refreshToken);), and call SaveChangesAsync() in your service layer after all related operations are ready.
   
   Immediate Save in Special Cases:
   Only do it inside the repository if:
   
   The operation must be atomic and immediately persisted for security reasons.
   There are no other related changes in the same transaction.
   
   
   
Enterprise Guidance
   Keep your code simple and readable.
   If the framework expects the full entity, load it directly—don’t over-optimize prematurely.
   If you see performance issues in production,
   profile and optimize (e.g., minimize navigation properties or use a lightweight User entity).
 
 */
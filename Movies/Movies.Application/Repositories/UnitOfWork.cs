using Movies.Application.Data;

namespace Movies.Application.Repositories;

public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await db.SaveChangesAsync(cancellationToken);
    }
}
namespace Movies.Application.Data.Repositories;

internal sealed class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await db.SaveChangesAsync(cancellationToken);
    }
}
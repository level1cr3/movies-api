namespace Movies.Application.Data.Repositories;

internal sealed class UnitOfWork(ApplicationDbContext db) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await db.SaveChangesAsync(cancellationToken);
    }
}

/*
 Why not returning bool from this method save changes method
 
 What Do Enterprise Apps Do?
   Most codebases return int by default (matches EF Core signature, more flexible).
   
   Because if they ever need rows affected count they won't have to change it everywhere.
   Pros: Lets caller know exactly how many rows were changed, 
   which can be useful for advanced scenarios (e.g., batch updates, auditing). 
 
 Should i name it completeAsync()
    stick with saveChangesAsync() : Familiarity: Developers coming from direct Entity Framework usage will instantly recognize it.
    we need to be pragmatic and realistic and make it more maintainable. not confusing. always remove confusion.
    move from distraction to simplicity. because simplicity is the ultimate sophistication
 Why?
   CompletedAsync sounds like a status or an event, not an operation.
   Developers expect SaveChangesAsync (or similar) to persist staged changes to the DB.
   Most ORMs, especially Entity Framework, use SaveChangesAsync as the convention.
 
 */
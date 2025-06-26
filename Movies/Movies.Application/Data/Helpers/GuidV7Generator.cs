using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Movies.Application.Data.Helpers;

internal class GuidV7Generator : ValueGenerator<Guid>
{
    public override Guid Next(EntityEntry entry)
    {
        return Guid.CreateVersion7();
    }

    public override bool GeneratesTemporaryValues => false;
}
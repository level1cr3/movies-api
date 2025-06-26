using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Movies.Application.Data.Helpers;

internal static class EntityConfigurationExtensions
{
    public static PropertyBuilder<Guid> ConfigureGuidV7PrimaryKey<T>(this EntityTypeBuilder<T> builder, 
        Expression<Func<T,Guid>> expression) where T : class
    {
        return builder.Property(expression)
            .HasValueGenerator<GuidV7Generator>()
            .ValueGeneratedOnAdd();
    }
    
}
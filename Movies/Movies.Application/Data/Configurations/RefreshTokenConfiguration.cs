using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Application.Data.Entities;
using Movies.Application.Data.Helpers;

namespace Movies.Application.Data.Configurations;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens").ConfigureGuidV7PrimaryKey(rt => rt.Id);
        
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(rt => rt.CreatedByIp).IsRequired().HasMaxLength(45);
        
        builder.Property(rt => rt.CreatedAt).IsRequired();

        builder.Property(rt => rt.ExpiresAt).IsRequired();

        builder.Property(rt => rt.RevokedByIp).HasMaxLength(45);
        
        builder.Property(rt => rt.RevokedReason).HasMaxLength(100);
        
        builder.HasOne(rt => rt.ReplacedByToken)
            .WithMany()
            .HasForeignKey(rt => rt.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
    }
}


/*
    builder.HasOne(rt => rt.User)
    This say: each refreshToken has one user.
    
    .withMany()
    This says: The related User can have many RefreshTokens.
    
    Because no collection is explicitly defined here, it assumes:
    You did not add a RefreshTokens collection to your ApplicationUser class.
    
    If you did add the following property to your ApplicationUser class:
    
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    
    Then you should use:
    .WithMany(u => u.RefreshTokens)
    Which tells EF Core to map the inverse side of the relationship — allowing navigation from user to their tokens.
    
    .HasForeignKey(rt => rt.UserId)
    This maps the foreign key from RefreshToken to the User:
    "Use the UserId property on the RefreshToken entity as the foreign key to link to the primary key of User."
 
 */
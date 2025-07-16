using System.ComponentModel.DataAnnotations;

namespace Movies.Application.Data.Entities;

internal sealed class RefreshToken
{
    public Guid Id { get; init; } = Guid.CreateVersion7(); 

    [MaxLength(256)]
    public required string Token { get; init; }
    
    [MaxLength(45)]
    public required string CreatedByIp { get; init; }
    
    public required DateTime CreatedAt { get; init; }
    
    public required DateTime ExpiresAt { get; init; }

    public bool IsRevoked { get; init; }
    
    public DateTime? RevokedAt { get; init; }
    
    [MaxLength(45)]
    public string? RevokedByIp { get; init; }

    [MaxLength(100)]
    public string? RevokedReason { get; init; }

    
    public Guid? ReplacedByTokenId { get; init; }
    public RefreshToken? ReplacedByToken { get; init; }
    
    public required Guid UserId { get; init; }
    public ApplicationUser User { get; init; } = null!;

}

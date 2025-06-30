using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Movies.Application.Data.Entities;

internal class ApplicationUser : IdentityUser<Guid>
{
    [MaxLength(200)]
    public required string FirstName { get; set; }

    [MaxLength(200)] 
    public string? LastName { get; set; } = null;

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    public DateTime? DateUpdated { get; set; }
}
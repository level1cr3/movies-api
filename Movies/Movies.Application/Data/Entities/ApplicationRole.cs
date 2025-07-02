using Microsoft.AspNetCore.Identity;

namespace Movies.Application.Data.Entities;

internal class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base()
    {
        
    }
    public ApplicationRole(string role) : base(role)
    {
        
    }
}
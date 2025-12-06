using Microsoft.AspNetCore.Identity;

namespace PropuestaTecnica.Common.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
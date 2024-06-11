using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class AppUser : IdentityUser
{
    public IEnumerable<IdentityRole>? Roles { get; set; }
}

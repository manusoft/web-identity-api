using Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class SeedData
{
    private static readonly IEnumerable<SeedUser> seedUsers =
   [
       new SeedUser()
        {
            Email = "manoj@test.com",
            NormalizedEmail = "MANOJ@TEST.COM",
            NormalizedUserName = "MANOJ@TEST.COM",
            RoleList = [ "Administrator", "Manager" ],
            UserName = "manoj@test.com",
            Password = "Passw0rd!"

        },
        new SeedUser()
        {
            Email = "binoj@test.com",
            NormalizedEmail = "BINOJ@TEST.COM",
            NormalizedUserName = "BINOJ@TEST.COM",
            RoleList = [ "User" ],
            UserName = "binoj@test.com",
            Password = "Passw0rd!"
        },
    ];

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        if (context.Users.Any())
        {
            return;
        }

        var userStore = new UserStore<AppUser>(context);
        var password = new PasswordHasher<AppUser>();

        using var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = ["Administrator", "Manager", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        foreach (var user in seedUsers)
        {
            var hashed = password.HashPassword(user, user.Password);
            user.PasswordHash = hashed;
            await userStore.CreateAsync(user);

            if (user.Email is not null)
            {
                var appUser = await userManager.FindByEmailAsync(user.Email);

                if (appUser is not null && user.RoleList is not null)
                {
                    await userManager.AddToRolesAsync(appUser, user.RoleList);
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private class SeedUser : AppUser
    {
        public string Password { get; set; }
        public string[]? RoleList { get; set; }
    }
}

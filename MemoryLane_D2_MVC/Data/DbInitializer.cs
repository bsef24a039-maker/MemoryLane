using MemoryLane_D2_MVC.Models;
using Microsoft.AspNetCore.Identity;

namespace MemoryLane_D2_MVC.Data;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        foreach (var role in new[] { "Admin", "User" })
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));

        // Demo admin is created only if this email does not already exist.
        const string adminEmail = "admin@memorylane.local";
        var admin = await users.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FullName = "MemoryLane Administrator", EmailConfirmed = true };
            var result = await users.CreateAsync(admin, "Admin@123");
            if (result.Succeeded) await users.AddToRoleAsync(admin, "Admin");
        }
    }
}

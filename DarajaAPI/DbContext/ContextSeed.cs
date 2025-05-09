using DarajaAPI.Enums;
using DarajaAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;

public static class ContextSeed
{
    public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        try
        {
            if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            }
            if (!await roleManager.RoleExistsAsync(Roles.Admin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }
            if (!await roleManager.RoleExistsAsync(Roles.Staff.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Staff.ToString()));
            }
            if (!await roleManager.RoleExistsAsync(Roles.Tenant.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Tenant.ToString()));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding roles: {ex.Message}");
        }
    }

    public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        try
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "mweshiols",
                Email = "mweshiols@gmail.com",
                FirstName = "Ian",
                LastName = "Gatumu",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Email != defaultUser.Email))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    var result = await userManager.CreateAsync(defaultUser, "Admin123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(defaultUser, Roles.Tenant.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.Staff.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                    }
                    else
                    {
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding super admin: {ex.Message}");
        }
    }
}
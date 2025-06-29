using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.IntegrationTesting.Data
{
    public static class TestDataSeeder
    {
        public static async Task SeedTestUserAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var testEmail = "testuser@example.com";
            var testPassword = "strongPassword@123!";

            if (!await userManager.Users.AnyAsync(u => u.Email == testEmail))
            {
                var testUser = new ApplicationUser
                {
                    UserName = testEmail,
                    Email = testEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(testUser, testPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Test user creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                if (!await roleManager.RoleExistsAsync("SuperUser"))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = "SuperUser" });
                }

                await userManager.AddToRoleAsync(testUser, "SuperUser");
            }
        }
    }
}

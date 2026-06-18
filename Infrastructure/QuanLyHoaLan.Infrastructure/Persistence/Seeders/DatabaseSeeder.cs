using Microsoft.EntityFrameworkCore;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Add roles if they don't exist
        if (!await context.Roles.AnyAsync(r => r.Code == "ADMIN"))
        {
            await context.Roles.AddAsync(new Role 
            { 
                Code = "ADMIN", 
                Name = RoleConstants.Admin, 
                Description = "Administrator Role" 
            });
        }

        if (!await context.Roles.AnyAsync(r => r.Code == "USER"))
        {
            await context.Roles.AddAsync(new Role 
            { 
                Code = "USER", 
                Name = RoleConstants.User, 
                Description = "Default User Role" 
            });
        }

        await context.SaveChangesAsync();

        // Seed default Admin User
        var adminEmail = "duongthanhson2004@gmail.com";
        if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "ADMIN");
            if (adminRole != null)
            {
                var adminUser = new User
                {
                    Email = adminEmail,
                    FullName = "System Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RoleId = adminRole.Id
                };
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}

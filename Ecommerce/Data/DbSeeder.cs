using Microsoft.AspNetCore.Identity;
using Ecommerce.Constants;


namespace Ecommerce.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();
            //Adding some roles to db
            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            //create admin user

            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };
            var userInDb = await userMgr.FindByEmailAsync(admin.Email);
            if (userInDb == null)
            {
                var creationResult = await userMgr.CreateAsync(admin, "Admin123");
                if (!creationResult.Succeeded)
                {
                    foreach (var error in creationResult.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
                else
                {
                    await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
                }
            }

        }

    }
}

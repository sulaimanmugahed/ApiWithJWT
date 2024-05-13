using ApiWithJWT.Models;
using Microsoft.AspNetCore.Identity;


namespace ApiWithJWT.Data.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<Role> roleManager)
        {
            //Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new Role("Admin"));

			
			if (!await roleManager.RoleExistsAsync("User"))
				await roleManager.CreateAsync(new Role("User"));
		}
    }
}

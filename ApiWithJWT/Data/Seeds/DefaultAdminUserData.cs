using ApiWithJWT.Models;
using Microsoft.AspNetCore.Identity;


namespace ApiWithJWT.Data.Seeds;

public class DefaultAdminUserData
{
	public static async Task SeedAsync(UserManager<User> userManager)
	{

		var defaultUser = new User
		{
			UserName = "sulaimanmugahed",
			Email = "sulaimanmugahed@gmail.com",
			Name = "Sulaiman Mugahed",
			PhoneNumber = "00967773050577",
			EmailConfirmed = true,
			PhoneNumberConfirmed = true
		};

		if (userManager.Users.All(u => u.Id != defaultUser.Id))
		{
			var user = await userManager.FindByEmailAsync(defaultUser.Email);
			if (user == null)
			{
				await userManager.CreateAsync(defaultUser, "myPassword");
				await userManager.AddToRoleAsync(defaultUser, "Admin");
			}

		}
	}
}

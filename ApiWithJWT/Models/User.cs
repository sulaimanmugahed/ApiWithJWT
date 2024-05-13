using Microsoft.AspNetCore.Identity;

namespace ApiWithJWT.Models;

public class User:IdentityUser
{
	public string Name { get; set; }
}

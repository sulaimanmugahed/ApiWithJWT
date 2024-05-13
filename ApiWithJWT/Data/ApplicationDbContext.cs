﻿using ApiWithJWT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiWithJWT.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	:IdentityDbContext<User,Role,string>(options)
{
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<User>(entity =>
		{
			entity.ToTable("Users");
		});

		builder.Entity<Role>(entity =>
		{
			entity.ToTable("Roles");
		});


		builder.Entity<IdentityUserRole<string>>(entity =>
		{
			entity.ToTable("UserRoles");
		});

		builder.Entity<IdentityUserClaim<string>>(entity =>
		{
			entity.ToTable("UserClaims");
		});

		builder.Entity<IdentityUserLogin<string>>(entity =>
		{
			entity.ToTable("UserLogins");
		});

		builder.Entity<IdentityRoleClaim<string>>(entity =>
		{
			entity.ToTable("RoleClaims");

		});

		builder.Entity<IdentityUserToken<string>>(entity =>
		{
			entity.ToTable("UserTokens");
		});
	}
}

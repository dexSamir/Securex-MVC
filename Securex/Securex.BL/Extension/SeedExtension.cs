using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Securex.Core.Entities;
using Securex.Core.Enums;

namespace Securex.BL.Extension;
public static class SeedExtension
{
	public static void UseUserSeed(this IApplicationBuilder app)
	{
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			if(!roleManager.Roles.Any())
			{
				foreach (var role in Enum.GetValues(typeof(Roles)))
				{
					roleManager.CreateAsync(new IdentityRole(role.ToString())).Wait(); 
				}
			}
			if(!userManager.Users.Any(x => x.NormalizedUserName == "ADMIN"))
			{
				User user = new User
				{
					Fullname = "Admin",
					UserName = "admin",
					Email = "admin@gmail.com"
				};

				userManager.CreateAsync(user, "admin123").Wait();
				userManager.AddToRoleAsync(user, nameof(Roles.Admin)).Wait(); 
			}
		}
	}
}


using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Securex.Core.Entities;

namespace Securex.DAL.Context;
public class AppDbContext : IdentityDbContext<User>
{
	public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments{ get; set; }

    public AppDbContext(DbContextOptions opt) : base(opt)
	{

	}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
        base.OnModelCreating(builder);
    }
}


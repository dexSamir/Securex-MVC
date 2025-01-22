using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Securex.Core.Entities;

namespace Securex.DAL.Configuration;
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(x => x.Fullname)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(512);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull); 
    }
}


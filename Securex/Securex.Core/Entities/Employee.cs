using System;
using Securex.Core.Entities.Base;

namespace Securex.Core.Entities;
public class Employee : BaseEntity
{
	public string Fullname { get; set; }
	public string? ImageUrl { get; set; }
	public int? DepartmentId { get; set; }
	public Department? Department { get; set; }
}


using System;
using Securex.Core.Entities.Base;

namespace Securex.Core.Entities;
public class Department : BaseEntity	
{
	public string Name { get; set; }
	public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>(); 
}


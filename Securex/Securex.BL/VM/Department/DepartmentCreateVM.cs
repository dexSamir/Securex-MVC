using System;
using System.ComponentModel.DataAnnotations;

namespace Securex.BL.VM.Department;
public class DepartmentCreateVM
{
	[Required(ErrorMessage = "Name is required"), MaxLength(64, ErrorMessage = "Name length must be less than 64 charachters")]
	public string Name { get; set; }
}


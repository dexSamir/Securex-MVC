using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Securex.BL.VM.Employee;
public class EmployeeCreateVM
{
	[Required(ErrorMessage = "Fullname is required"), MaxLength(128, ErrorMessage = "Fullname length must be less than 128 charachters")]
    public string Fullname { get; set; }
    [Required]
    public IFormFile? Image { get; set; }
    [Required]
    public int? DepartmentId { get; set; }
}


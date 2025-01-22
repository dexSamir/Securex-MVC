using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Securex.BL.VM.Employee;
public class EmployeeUpdateVM
{
    [Required(ErrorMessage = "Fullname is required"), MaxLength(128, ErrorMessage = "Fullname length must be less than 128 charachters")]
    public string Fullname { get; set; }
    [Required]
    public IFormFile? Image { get; set; }
    public string? ExistingImageUrl{ get; set; }
    [Required]
    public int? DepartmentId { get; set; }
}


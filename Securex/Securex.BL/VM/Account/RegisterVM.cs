using System;
using System.ComponentModel.DataAnnotations;

namespace Securex.BL.VM.Account;
public class RegisterVM
{
	[Required, MaxLength(64)]
	public string Username { get; set; }
    [Required, MaxLength(128)]
    public string Email { get; set; }
    [Required, MaxLength(128)]
    public string Fullname { get; set; }
    [Required, MaxLength(32), DataType(DataType.Password)]
    public string Password { get; set; }
    [Required, MaxLength(32), DataType(DataType.Password), Compare(nameof(Password))]
    public string RePassword { get; set; }
}


using System;
using System.ComponentModel.DataAnnotations;

namespace Securex.BL.VM.Account;
public class LoginVM
{
	[Required, MaxLength(128)]
	public string EmailOrUsername { get; set; }
	[Required, MaxLength(32), DataType(DataType.Password)]
	public string Password { get; set; }
	[Required]
	public bool RememberMe { get; set; }
}


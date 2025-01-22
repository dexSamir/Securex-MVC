using System;
using Microsoft.AspNetCore.Identity;

namespace Securex.Core.Entities;
public class User : IdentityUser
{
	public string Fullname { get; set;  }
}


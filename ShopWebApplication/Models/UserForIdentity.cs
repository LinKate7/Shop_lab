using System;
using Microsoft.AspNetCore.Identity;

namespace ShopWebApplication.Models
{
	public class UserForIdentity : IdentityUser<int>
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Year { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }
    }
}


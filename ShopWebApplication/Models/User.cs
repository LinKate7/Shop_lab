﻿using Microsoft.AspNetCore.Identity;

namespace ShopWebApplication.Models;

public partial class User 
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName => FirstName + " " + LastName;

    public int Year { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }
}

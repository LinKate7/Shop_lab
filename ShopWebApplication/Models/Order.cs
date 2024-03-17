using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopWebApplication.Models;

public partial class Order
{
    public int OrderId { get; set; }

    [Display(Name ="ID користувача")]
    public int UserId { get; set; }

    [Display(Name ="Дата й час")]
    public DateTime OrderDate { get; set; }

    [Display(Name = "До оплати")]
    public int Price { get; set; }

    [Display(Name = "Статус")]
    public int StatusId { get; set; }

    public int ClientInfoId { get; set; }

    [Display(Name = "Одержувач")]
    public virtual ClientInfo? ClientInfo { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [Display(Name = "Статус")]
    public virtual Status? Status { get; set; }

    [Display(Name = "Користувач, що замовив")]
    public virtual User? User { get; set; }
}

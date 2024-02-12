using System;
using System.Collections.Generic;

namespace ShopWebApplication.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Price { get; set; }

    public int StatusId { get; set; }

    public int ClientInfoId { get; set; }

    public virtual ClientInfo ClientInfo { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Status Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

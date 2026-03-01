using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class Order
{
    public int Orderid { get; set; }

    public DateOnly? DateOrder { get; set; }

    public DateOnly? DateDeliv { get; set; }

    public int? PickUpPointId { get; set; }

    public int? FullName { get; set; }

    public int? Code { get; set; }

    public int? OrderStatusId { get; set; }

    public virtual User? FullNameNavigation { get; set; }

    public virtual OrderStatus? OrderStatus { get; set; }

    public virtual PickUpPoint? PickUpPoint { get; set; }

    public virtual ICollection<TovarInOrder> TovarInOrders { get; set; } = new List<TovarInOrder>();
}

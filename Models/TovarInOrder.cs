using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class TovarInOrder
{
    public int TovarInOrderId { get; set; }

    public int? OrderId { get; set; }

    public int? TovarId { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Tovar? Tovar { get; set; }
}

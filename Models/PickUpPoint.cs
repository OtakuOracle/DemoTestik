using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class PickUpPoint
{
    public int PickUpPointId { get; set; }

    public string? PickUpPointName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

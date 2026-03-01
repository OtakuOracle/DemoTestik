using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public string? UnitName { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}

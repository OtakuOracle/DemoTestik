using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class TovarType
{
    public int TovarTypeId { get; set; }

    public string? TovarTypeName { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}

using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string? ManufacturerName { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}

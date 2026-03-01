using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}

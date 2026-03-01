using System;
using System.Collections.Generic;

namespace DemoTest.Models;

public partial class Provider
{
    public int ProviderId { get; set; }

    public string? ProviderName { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}

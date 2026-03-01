using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace DemoTest.Models;

public partial class Tovar
{
    public int TovarId { get; set; }

    public string? Art { get; set; }

    public int? TovarTypeId { get; set; }

    public int? Price { get; set; }

    public int? ManufacturerId { get; set; }

    public int? CategoryId { get; set; }

    public int? DiscountNow { get; set; }

    public int? Unit { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public string? Photo { get; set; }

    public Bitmap GetPhoto
    {
        get
        {
            if (Photo != null && Photo != "")
            {
                return new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "/" + Photo);
            }
            else
            {
                return new Bitmap(AppDomain.CurrentDomain.BaseDirectory + "/images/11.jpeg");
            }
        }
    }

    public int? ProviderId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<TovarInOrder> TovarInOrders { get; set; } = new List<TovarInOrder>();

    public virtual TovarType? TovarType { get; set; }

    public virtual Unit? UnitNavigation { get; set; }
}

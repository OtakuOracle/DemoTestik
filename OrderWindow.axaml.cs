using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DemoTest.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoTest;

public partial class OrderWindow : Window
{
    public OrderWindow()
    {
        InitializeComponent();
        Get();
    }


    private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var catalogWindow = new CatalogWindow();
        catalogWindow.Show();
        this.Close();
    }


    private void Get()
    {
        using var context = new TrenirovkaContext();

        var allOrders = context.Orders
                                .Include(x => x.OrderStatus)
                                .Include(x => x.PickUpPoint)
                                .Include(x => x.TovarInOrders) // Добавляем включение для связи с tovar_in_order
                                .ThenInclude(tio => tio.Tovar)
                                .ToList();

        OrdersBox.ItemsSource = allOrders;
    }



    private void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var addeditorder = new AddEditOrder();
        addeditorder.Show();
        this.Close();
    }

    private void OrdersBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {

        if (OrdersBox.SelectedItem is Order order)
        {
            var addedit = new AddEditProduct();
            addedit.Show();
            this.Close();
        }
    }

}
using Avalonia.Controls;
using MsBox.Avalonia;
using System.Linq;
using DemoTest.Models;

namespace DemoTest;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Exit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

    private void Guest_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var catalog = new CatalogWindow(0);
        catalog.Show();
        this.Close();
    }

    private async void Auth_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var context = new TrenirovkaContext();

        var login = LoginBox.Text;
        var password = passwordBox.Text;

        var user = context.Users.FirstOrDefault(x => x.Login == login && x.Password == password);

        if (user != null)
        {
            var catalog = new CatalogWindow(user.UserId);
            catalog.Show();
            this.Close();
        }
        else
        {
            var error = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Вы ввели неверные данные", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            error.ShowAsync();
        }
    }
}
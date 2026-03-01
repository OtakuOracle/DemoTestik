using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DemoTest.Models;

namespace DemoTest;

public partial class CatalogWindow : Window
{

    private int _currentUserId;
    public CatalogWindow()
    {
        InitializeComponent();
        LoadBox();
        Get();

    }

    public CatalogWindow(int userid)
    {
        InitializeComponent();
        _currentUserId = userid;
        CheckRole();
        LoadBox();
        Get();


    }

    private void CheckRole()
    {
        using var context = new TrenirovkaContext();

        var curUser = context.Users.Where(x => x.UserId == _currentUserId).FirstOrDefault();
        if (curUser.RoleId == 2 || curUser.RoleId == 3)

        {
            AddButton.IsVisible = true;
        }
    }

    private void LoadBox()
    {
        using var context = new TrenirovkaContext();

        var manufacturer = context.Manufacturers.Select(x => x.ManufacturerName).ToList();

        manufacturer.Add("Все производители");

        Filter.ItemsSource = manufacturer.OrderByDescending(x => x == "Все производители");

        Filter.SelectedIndex = 0;


    }

    private void Get()
    {
        using var context = new TrenirovkaContext();

        // Включаем TovarType, Category и Manufacturer при загрузке
        var allProducts = context.Tovars
                                .Include(x => x.Category)
                                .Include(x => x.Manufacturer)
                                .Include(x => x.TovarType)
                                .Include(x => x.Provider)
                                .ToList();

        switch (Sort.SelectedIndex)
        {
            case 0: // Сортировка по возрастанию цены
                allProducts = allProducts.OrderBy(x => x.Price).ToList();
                break;
            case 1: // Сортировка по убыванию цены
                allProducts = allProducts.OrderByDescending(x => x.Price).ToList();
                break;
            default: // Если не выбрано, сортируем по возрастанию
                allProducts = allProducts.OrderBy(x => x.Price).ToList();
                break;
        }

        // Фильтрация по производителю
        if (Filter.SelectedItem != null && Filter.SelectedItem.ToString() != "Все производители")
        {
            allProducts = allProducts.Where(x => x.Manufacturer.ManufacturerName == Filter.SelectedItem.ToString()).ToList();
        }


        if (SearchBox != null && !string.IsNullOrWhiteSpace(SearchBox.Text))
        {
            var searchTerm = SearchBox.Text.ToLower();
            allProducts = allProducts.Where(x =>
                // Ищем в наименовании типа товара
                (x.TovarType != null && !string.IsNullOrWhiteSpace(x.TovarType.TovarTypeName) && x.TovarType.TovarTypeName.ToLower().Contains(searchTerm)) ||
                // Или в наименовании категории
                (x.Category != null && !string.IsNullOrWhiteSpace(x.Category.CategoryName) && x.Category.CategoryName.ToLower().Contains(searchTerm)) ||
                // Или в наименовании производителя
                (x.Manufacturer != null && !string.IsNullOrWhiteSpace(x.Manufacturer.ManufacturerName) && x.Manufacturer.ManufacturerName.ToLower().Contains(searchTerm)) ||
                
                (x.Provider != null && !string.IsNullOrWhiteSpace(x.Provider.ProviderName) && x.Provider.ProviderName.ToLower().Contains(searchTerm))

            ).ToList();
        }



        TovarsBox.ItemsSource = allProducts;
    }

    private void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var addedit = new AddEditProduct(_currentUserId);
        addedit.Show();
        this.Close();
    }

    private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var main = new MainWindow();
        main.Show();
        this.Close();
    }

    private void SearchBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        Get(); // Обновляем каталог при вводе текста в SearchBox
    }

    private void Sort_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Get(); // Обновляем каталог при изменении сортировки
    }

    private void Filter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        Get(); // Обновляем каталог при изменении фильтра

    }

    private void TovarsBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TovarsBox.SelectedItem is Tovar tovar)
        {
            var addedit = new AddEditProduct(_currentUserId, tovar);
            addedit.Show();
            this.Close();
        }
    }
}

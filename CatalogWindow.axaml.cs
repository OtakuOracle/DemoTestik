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
        // 1. Сначала проверяем, не зашел ли пользователь как Гость (ID = 0)
        if (_currentUserId == 0)
        {
            AddButton.IsVisible = false;
            Filter.IsVisible = false;
            Sort.IsVisible = false;
            SearchBox.IsVisible = false;
            return; // Выходим из метода, так как гостя нет в базе данных
        }

        using var context = new TrenirovkaContext();
        var curUser = context.Users.FirstOrDefault(x => x.UserId == _currentUserId);

        if (curUser != null)
        {
            // только для Администратора - Роль 1 он может добавить удалить и редактировать товар
            if (curUser.RoleId == 1)
            {
                AddButton.IsVisible = true;
            }
            else
            {
                AddButton.IsVisible = false;
            }

            // Управление поиском, фильтрацией и сортировкой
            if (curUser.RoleId == 1 || curUser.RoleId == 2)
            {
                // Админ (1) и Пользователь (2) могут пользоваться инструментами
                Filter.IsVisible = true;
                Sort.IsVisible = true;
                SearchBox.IsVisible = true;
            }
            else if (curUser.RoleId == 3)
            {
                // Роль 3 (Просмотр) — скрываем инструменты
                Filter.IsVisible = false;
                Sort.IsVisible = false;
                SearchBox.IsVisible = false;
            }
            else
            {
                // Для любых других ролей на всякий случай тоже скрываем
                Filter.IsVisible = false;
                Sort.IsVisible = false;
                SearchBox.IsVisible = false;
            }
        }
    }


    private void LoadBox() //для комбобокса
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
        // Если роль НЕ 1 (не админ), просто выходим из метода
        if (_currentUserId != 1)
        {
            return;
        }

        // Если админ — переходим к редактированию
        if (TovarsBox.SelectedItem is Tovar tovar)
        {
            var addedit = new AddEditProduct(_currentUserId, tovar);
            addedit.Show();
            this.Close();
        }
    }

}

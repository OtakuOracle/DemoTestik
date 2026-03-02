using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using System;
using System.IO;
using System.Linq;
using DemoTest.Models;
using System.Collections.Generic; // Добавим для List<string>

namespace DemoTest;

public partial class AddEditProduct : Window
{
    private Tovar _tovar;
    private int _currentUser;
    private string ImageName = Guid.NewGuid().ToString("N");

    public AddEditProduct()
    {
        InitializeComponent();
    }
    public AddEditProduct(int currentUserId)
    {
        InitializeComponent();
        _currentUser = currentUserId;
        LoadManu();
        LoadPro();
        LoadCat();
        LoadUnit(); // Добавим вызов для загрузки единиц измерения
        DataContext = new Tovar();
    }

    public AddEditProduct(int currentUserId, Tovar tovar)
    {
        InitializeComponent();
        using var context = new TrenirovkaContext();
        _currentUser = currentUserId;
        _tovar = tovar;
        LoadManu();
        LoadPro();
        LoadCat();
        LoadUnit(); // Добавим вызов для загрузки единиц измерения
        DataContext = _tovar;
        EditBut.IsVisible = true;
        DeleteBut.IsVisible = true;
        ImageBox.Source = new Bitmap(_tovar.Photo);
        var a = _tovar.ManufacturerId;
        var b = _tovar.CategoryId;
        var c = _tovar.ProviderId;
        var d = _tovar.Unit; // Получаем UnitId

        Provider.SelectedItem = context.Providers.Where(x => x.ProviderId == c).Select(x => x.ProviderName).FirstOrDefault();
        Manufacturer.SelectedItem = context.Manufacturers.Where(x => x.ManufacturerId == a).Select(x => x.ManufacturerName).FirstOrDefault();
        Category.SelectedItem = context.Categories.Where(x => x.CategoryId == b).Select(x => x.CategoryName).FirstOrDefault();
        Unit.SelectedItem = context.Units.Where(x => x.UnitId == d).Select(x => x.UnitName).FirstOrDefault(); // Устанавливаем выбранную единицу измерения
    }

    private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var catalog = new CatalogWindow(_currentUser);
        catalog.Show();
        this.Close();
    }

    private async void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            using var context = new TrenirovkaContext();
            var newTovar = DataContext as Tovar;

            if (Manufacturer.SelectedItem != null && Provider.SelectedItem != null && Category.SelectedItem != null && Unit.SelectedItem != null) // Добавим проверку Unit.SelectedItem
            {
                var man = Manufacturer.SelectedItem.ToString();
                var pro = Provider.SelectedItem.ToString();
                var cat = Category.SelectedItem.ToString();
                var unit = Unit.SelectedItem.ToString(); // Получаем выбранную единицу измерения

                var manFin = context.Manufacturers.Where(x => x.ManufacturerName == man).Select(x => x.ManufacturerId).FirstOrDefault();
                var proFin = context.Providers.Where(x => x.ProviderName == pro).Select(x => x.ProviderId).FirstOrDefault();
                var catFin = context.Categories.Where(x => x.CategoryName == cat).Select(x => x.CategoryId).FirstOrDefault();
                var unitFin = context.Units.Where(x => x.UnitName == unit).Select(x => x.UnitId).FirstOrDefault(); // Находим UnitId по UnitName

                newTovar.ProviderId = proFin;
                newTovar.ManufacturerId = manFin;
                newTovar.CategoryId = catFin;
                newTovar.Unit = unitFin; // Присваиваем UnitId
                newTovar.Photo = ImageName;

                context.Tovars.Add(newTovar);
                await context.SaveChangesAsync();

                var nice = MessageBoxManager.GetMessageBoxStandard("Успех", "Товар создан", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success);
                await nice.ShowAsync();

                var catalog = new CatalogWindow(_currentUser);
                catalog.Show();
                this.Close();
            }
            else
            {
                // Добавим сообщение об ошибке, если какие-то поля не заполнены
                var error = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Пожалуйста, заполните все поля, включая единицу измерения.", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
                await error.ShowAsync();
            }
        }
        catch (Exception ex)
        {
            var excep = ex.ToString();
            var error = MessageBoxManager.GetMessageBoxStandard("Ошибка", excep, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            error.ShowAsync();
        }
    }

    private void LoadManu()
    {
        using var context = new TrenirovkaContext();
        var man = context.Manufacturers.Select(x => x.ManufacturerName).ToList();
        Manufacturer.ItemsSource = man;
    }
    private void LoadPro()
    {
        using var context = new TrenirovkaContext();
        var sup = context.Providers.Select(x => x.ProviderName).ToList();
        Provider.ItemsSource = sup;
    }
    private void LoadCat()
    {
        using var context = new TrenirovkaContext();
        var cat = context.Categories.Select(x => x.CategoryName).ToList();
        Category.ItemsSource = cat;
    }

    // Новый метод для загрузки единиц измерения
    private void LoadUnit()
    {
        using var context = new TrenirovkaContext();
        var units = context.Units.Select(x => x.UnitName).ToList();
        Unit.ItemsSource = units; // Привязываем список к ComboBox
    }

    private async void AddImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
        {
            Title = "Добавить кратинку",
            FileTypeChoices = new[]
            {
                FilePickerFileTypes.All
            }
        });

        if (file != null)
        {
            ImageBox.Source = new Bitmap(file.Path.LocalPath);
            var targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImageName + Path.GetExtension(file.Name));
            File.Copy(file.Path.LocalPath, targetPath);
            ImageName = targetPath;
        }
    }

    private async void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var context = new TrenirovkaContext();

        var tovarId = _tovar.TovarId;

        var tovarToDelete = context.Tovars.Where(x => x.TovarId == tovarId).FirstOrDefault();

        if (tovarToDelete != null)
        {
            context.Remove(tovarToDelete);
            context.SaveChanges();
        }

        var nice = MessageBoxManager.GetMessageBoxStandard("Успех", "Товар удален", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success);
        await nice.ShowAsync();

        var catalog = new CatalogWindow(_currentUser);
        catalog.Show();
        this.Close();
    }

    private async void Edit_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using var context = new TrenirovkaContext();

        try
        {
            var man = Manufacturer.SelectedItem.ToString();
            var pro = Provider.SelectedItem.ToString();
            var cat = Category.SelectedItem.ToString();
            var unit = Unit.SelectedItem.ToString(); // Получаем выбранную единицу измерения

            var manFin = context.Manufacturers.Where(x => x.ManufacturerName == man).Select(x => x.ManufacturerId).FirstOrDefault();
            var supFin = context.Providers.Where(x => x.ProviderName == pro).Select(x => x.ProviderId).FirstOrDefault();
            var catFin = context.Categories.Where(x => x.CategoryName == cat).Select(x => x.CategoryId).FirstOrDefault();
            var unitFin = context.Units.Where(x => x.UnitName == unit).Select(x => x.UnitId).FirstOrDefault(); // Находим UnitId по UnitName

            _tovar.ProviderId = supFin;
            _tovar.ManufacturerId = manFin;
            _tovar.CategoryId = catFin;
            _tovar.Unit = unitFin; // Присваиваем UnitId
            _tovar.Photo = ImageName;

            context.Tovars.Update(_tovar);
            await context.SaveChangesAsync();

            var nice = MessageBoxManager.GetMessageBoxStandard("Успех", "Товар изменен", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Success);
            await nice.ShowAsync();

            var catalog = new CatalogWindow(_currentUser);
            catalog.Show();
            this.Close();

        }
        catch (Exception ex)
        {
            var exec = ex.ToString();
            var error = MessageBoxManager.GetMessageBoxStandard("Ошибка", exec, MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            error.ShowAsync();
        }

        // Этот код был дублирован и после try-catch, можно его удалить или перенести
        // var tovar = DataContext as Tovar;
        // context.SaveChanges();
    }
}

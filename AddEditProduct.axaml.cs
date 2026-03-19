using System;
using System.Collections.Generic; 
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using DemoTest.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace DemoTest;

public partial class AddEditProduct : Window
{
    private Tovar _tovar;
    private int _currentUser;
    private string ImageName;

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
        LoadTovarType();
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
        LoadTovarType();
        DataContext = _tovar;
        EditBut.IsVisible = true;
        DeleteBut.IsVisible = true;
        ImageBox.Source = _tovar.GetPhoto;
        var a = _tovar.ManufacturerId;
        var b = _tovar.CategoryId;
        var c = _tovar.ProviderId;
        var d = _tovar.Unit; // Получаем UnitId
        var e = _tovar.TovarTypeId; //из таблицы товаров все 

        TovarType.SelectedItem = context.TovarTypes.Where(x => x.TovarTypeId == e).Select(x => x.TovarTypeName).FirstOrDefault();
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


    private bool ValidateProduct(Tovar t)
    {
        // Price: не может быть отрицательным 
        if (t.Price.HasValue && t.Price < 0)
        {
            var errorPrice = MessageBoxManager.GetMessageBoxStandard(
                "Ошибка",
                "цена не должна быть отрицательной",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            errorPrice.ShowAsync();
            return false;
        }

        // Quantity: не может быть отрицательным 
        if (t.Quantity.HasValue && t.Quantity < 0)
        {
            var errorQty = MessageBoxManager.GetMessageBoxStandard(
                "Ошибка",
                "количество не должно быть отрицательным",
                MsBox.Avalonia.Enums.ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            errorQty.ShowAsync();
            return false;
        }

        return true;
    }

    private async void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            using var context = new TrenirovkaContext();
            var newTovar = DataContext as Tovar;

            if (!ValidateProduct(newTovar))
            {
                return;
            }

            if (Manufacturer.SelectedItem != null && Provider.SelectedItem != null && Category.SelectedItem != null && Unit.SelectedItem != null && TovarType.SelectedItem != null)
            {
                var man = Manufacturer.SelectedItem.ToString();
                var pro = Provider.SelectedItem.ToString();
                var cat = Category.SelectedItem.ToString();
                var unit = Unit.SelectedItem.ToString();
                var tovartype = TovarType.SelectedItem.ToString();

                var manFin = context.Manufacturers.Where(x => x.ManufacturerName == man).Select(x => x.ManufacturerId).FirstOrDefault();
                var proFin = context.Providers.Where(x => x.ProviderName == pro).Select(x => x.ProviderId).FirstOrDefault();
                var catFin = context.Categories.Where(x => x.CategoryName == cat).Select(x => x.CategoryId).FirstOrDefault();
                var unitFin = context.Units.Where(x => x.UnitName == unit).Select(x => x.UnitId).FirstOrDefault();
                var tovartypeFin = context.TovarTypes.Where(x => x.TovarTypeName == tovartype).Select(x => x.TovarTypeId).FirstOrDefault();

                newTovar.ProviderId = proFin;
                newTovar.ManufacturerId = manFin;
                newTovar.CategoryId = catFin;
                newTovar.Unit = unitFin;
                newTovar.Photo = "images/" + ImageName;
                newTovar.TovarTypeId = tovartypeFin;

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

    private void LoadUnit()
    {
        using var context = new TrenirovkaContext();
        var unit = context.Units.Select(x => x.UnitName).ToList();
        Unit.ItemsSource = unit; 
    }


    private void LoadTovarType()
    {
        using var context = new TrenirovkaContext();
        var tovartype = context.TovarTypes.Select(x => x.TovarTypeName).ToList();
        TovarType.ItemsSource = tovartype; 
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
            ImageName = Guid.NewGuid().ToString() + ".png";
            var targetPath = AppDomain.CurrentDomain.BaseDirectory + "/images/" + ImageName;
            File.Copy(file.Path.LocalPath, targetPath);

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
            var unit = Unit.SelectedItem.ToString();
            var tovartype = TovarType.SelectedItem.ToString();

            var manFin = context.Manufacturers.Where(x => x.ManufacturerName == man).Select(x => x.ManufacturerId).FirstOrDefault();
            var supFin = context.Providers.Where(x => x.ProviderName == pro).Select(x => x.ProviderId).FirstOrDefault();
            var catFin = context.Categories.Where(x => x.CategoryName == cat).Select(x => x.CategoryId).FirstOrDefault();
            var unitFin = context.Units.Where(x => x.UnitName == unit).Select(x => x.UnitId).FirstOrDefault();
            var tovartypeFin = context.TovarTypes.Where(x => x.TovarTypeName == tovartype).Select(x => x.TovarTypeId).FirstOrDefault();

            _tovar.ProviderId = supFin;
            _tovar.ManufacturerId = manFin;
            _tovar.CategoryId = catFin;
            _tovar.Unit = unitFin;
            _tovar.Photo = "images/" + ImageName;
            _tovar.TovarTypeId = tovartypeFin;

            if (!ValidateProduct(_tovar))
            {
                return;
            }

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

    }
}

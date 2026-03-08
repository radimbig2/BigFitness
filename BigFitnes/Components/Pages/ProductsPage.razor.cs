using BigFitness.Models;
using BigFitness.Services;
using Microsoft.AspNetCore.Components;

namespace BigFitness.Components.Pages;

public partial class ProductsPage : ComponentBase
{
    [Inject] private ProductService ProductService { get; set; } = null!;

    private List<Product>? _products;
    private string _search = "";

    private List<Product> Filtered => _products == null ? [] :
        string.IsNullOrWhiteSpace(_search) ? _products :
        _products.Where(p => p.Name.Contains(_search.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

    private bool _showModal;
    private bool _isNew;
    private bool _duplicateWarning;
    private bool _showDeleteConfirm;
    private bool _busy;
    private string? _statusMessage;
    private bool _statusIsError;

    private int _editId;
    private string _editName = "";
    private double _editCalories;
    private double _editProteins;
    private double _editFats;
    private double _editCarbs;
    private double _editPortion = 100;

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        _products = await ProductService.GetAllProductsSorted();
    }

    private void OpenAdd()
    {
        _isNew = true;
        _editId = 0;
        _editName = "";
        _editCalories = 0;
        _editProteins = 0;
        _editFats = 0;
        _editCarbs = 0;
        _editPortion = 100;
        _duplicateWarning = false;
        _showModal = true;
    }

    private void OpenEdit(Product product)
    {
        _isNew = false;
        _editId = product.Id;
        _editName = product.Name;
        _editCalories = product.Calories;
        _editProteins = product.Proteins;
        _editFats = product.Fats;
        _editCarbs = product.Carbs;
        _editPortion = product.DefaultPortionGrams;
        _duplicateWarning = false;
        _showModal = true;
    }

    private void CloseModal()
    {
        _showModal = false;
        _duplicateWarning = false;
    }

    private async Task SaveProduct()
    {
        if (string.IsNullOrWhiteSpace(_editName))
            return;

        var duplicate = await ProductService.NameExists(_editName.Trim(), _editId);
        if (duplicate)
        {
            _duplicateWarning = true;
            return;
        }

        if (_isNew)
        {
            var newProduct = new Product(
                _editName.Trim(), _editCalories, _editProteins, _editFats, _editCarbs, _editPortion, isCustom: true);
            await ProductService.AddCustomProduct(newProduct);
        }
        else
        {
            var existing = _products!.First(p => p.Id == _editId);
            existing.Update(_editName.Trim(), _editCalories, _editProteins, _editFats, _editCarbs, _editPortion);
            await ProductService.UpdateProduct(existing);
        }

        _showModal = false;
        _duplicateWarning = false;
        await LoadProducts();
    }

    private async Task ConfirmDelete()
    {
        _showDeleteConfirm = false;
        _showModal = false;
        await ProductService.DeleteProduct(_editId);
        await LoadProducts();
    }

    private async Task ToggleFavorite(Product product)
    {
        await ProductService.ToggleFavorite(product.Id);
        await LoadProducts();
    }

    private async Task ExportProducts()
    {
        _busy = true;
        _statusMessage = null;
        try
        {
            var path = await MainThread.InvokeOnMainThreadAsync(() => ProductService.ExportToJsonAsync());
            _statusMessage = $"Saved: {path}";
            _statusIsError = false;
        }
        catch (Exception ex)
        {
            _statusMessage = $"Export error: {ex.Message}";
            _statusIsError = true;
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task ImportProducts()
    {
        _busy = true;
        _statusMessage = null;
        try
        {
            var result = await MainThread.InvokeOnMainThreadAsync(() =>
                FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, [".json"] }
                    })
                }));

            if (result == null)
            {
                _busy = false;
                return;
            }

            var json = await File.ReadAllTextAsync(result.FullPath);
            var importResult = await ProductService.ImportFromJsonAsync(json);
            _statusMessage = $"Imported: {importResult.Added} added, {importResult.Skipped} skipped";
            _statusIsError = false;
            await LoadProducts();
        }
        catch (Exception ex)
        {
            _statusMessage = $"Import error: {ex.Message}";
            _statusIsError = true;
        }
        finally
        {
            _busy = false;
        }
    }
}

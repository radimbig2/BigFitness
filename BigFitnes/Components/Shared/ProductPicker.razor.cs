using BigFitness.Models;
using BigFitness.Services;
using Microsoft.AspNetCore.Components;

namespace BigFitness.Components.Shared;

public partial class ProductPicker : ComponentBase
{
    [Inject] private ProductService ProductSvc { get; set; } = null!;

    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback<(int ProductId, double Grams)> OnProductAdded { get; set; }

    private List<Product> _displayProducts = new();
    private string _searchQuery = "";
    private bool _showFrequent = true;
    private Product? _selectedProduct;
    private double _portionGrams;
    private double _servingCount;

    protected override async Task OnParametersSetAsync()
    {
        if (IsVisible && !_displayProducts.Any())
            await LoadProducts();
    }

    private async Task LoadProducts()
    {
        _displayProducts = _showFrequent
            ? await ProductSvc.GetFrequentProducts(10)
            : await ProductSvc.GetAllProducts();
    }

    private async Task SearchAsync()
    {
        _displayProducts = string.IsNullOrWhiteSpace(_searchQuery)
            ? await (_showFrequent ? ProductSvc.GetFrequentProducts(10) : ProductSvc.GetAllProducts())
            : await ProductSvc.SearchProducts(_searchQuery);
    }

    private async Task SwitchTab(bool frequent)
    {
        _showFrequent = frequent;
        _searchQuery = "";
        await LoadProducts();
    }

    private void SelectProduct(Product product)
    {
        _selectedProduct = product;
        _servingCount = 1;
        _portionGrams = product.DefaultPortionGrams;
    }

    private void OnPortionChanged()
    {
        if (_selectedProduct is not null && _selectedProduct.DefaultPortionGrams > 0)
            _servingCount = Math.Round(_portionGrams / _selectedProduct.DefaultPortionGrams, 2);
    }

    private void AddServing()
    {
        _servingCount = Math.Round(_servingCount + 1, 2);
        _portionGrams = Math.Round(_selectedProduct!.DefaultPortionGrams * _servingCount, 1);
    }

    private void RemoveServing()
    {
        var next = Math.Round(_servingCount - 1, 2);
        if (next >= 0.01)
        {
            _servingCount = next;
            _portionGrams = Math.Round(_selectedProduct!.DefaultPortionGrams * _servingCount, 1);
        }
    }

    private async Task ConfirmAdd()
    {
        if (_selectedProduct is not null && _portionGrams > 0)
        {
            await OnProductAdded.InvokeAsync((_selectedProduct.Id, _portionGrams));
            _selectedProduct = null;
            await Close();
        }
    }

    private async Task Close()
    {
        _selectedProduct = null;
        _searchQuery = "";
        await OnClose.InvokeAsync();
    }
}

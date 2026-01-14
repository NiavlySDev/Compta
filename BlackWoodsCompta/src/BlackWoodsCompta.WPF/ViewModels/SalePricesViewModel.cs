using System.Collections.ObjectModel;
using System.Windows.Input;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class SalePricesViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    
    private ObservableCollection<SalePrice> _salePrices = new();
    private SalePrice? _selectedItem;
    private string _searchText = string.Empty;
    private string? _filterCategory;
    private bool _isLoading;
    private bool _isAddEditDialogOpen;
    
    // New/Edit item properties
    private string _newProductName = string.Empty;
    private string _newCategory = "Plat principal";
    private decimal _newPrice;
    private decimal _newCost;
    private string _newDescription = string.Empty;
    
    private bool _isEditing;
    private int _editingId;

    public ObservableCollection<SalePrice> SalePrices
    {
        get => _salePrices;
        set => SetProperty(ref _salePrices, value);
    }

    public SalePrice? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _ = LoadSalePricesAsync();
            }
        }
    }

    public string? FilterCategory
    {
        get => _filterCategory;
        set
        {
            if (SetProperty(ref _filterCategory, value))
            {
                _ = LoadSalePricesAsync();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsAddEditDialogOpen
    {
        get => _isAddEditDialogOpen;
        set => SetProperty(ref _isAddEditDialogOpen, value);
    }

    public string NewProductName
    {
        get => _newProductName;
        set => SetProperty(ref _newProductName, value);
    }

    public string NewCategory
    {
        get => _newCategory;
        set => SetProperty(ref _newCategory, value);
    }

    public decimal NewPrice
    {
        get => _newPrice;
        set
        {
            if (SetProperty(ref _newPrice, value))
            {
                UpdateMargin();
            }
        }
    }

    public decimal NewCost
    {
        get => _newCost;
        set
        {
            if (SetProperty(ref _newCost, value))
            {
                UpdateMargin();
            }
        }
    }

    public decimal NewMargin => NewPrice > 0 && NewCost > 0 ? ((NewPrice - NewCost) / NewPrice * 100) : 0;

    public string NewDescription
    {
        get => _newDescription;
        set => SetProperty(ref _newDescription, value);
    }

    public AsyncRelayCommand LoadSalePricesCommand { get; }
    public ICommand OpenAddDialogCommand { get; }
    public ICommand OpenEditDialogCommand { get; }
    public AsyncRelayCommand SaveItemCommand { get; }
    public ICommand CancelDialogCommand { get; }
    public AsyncRelayCommand DeleteItemCommand { get; }

    public List<string> Categories { get; } = new()
    {
        "Plat principal", "Entrée", "Dessert", "Boisson", "Accompagnement", "Menu"
    };

    public SalePricesViewModel(IDataService dataService, IAuthService authService)
    {
        _dataService = dataService;
        _authService = authService;

        LoadSalePricesCommand = new AsyncRelayCommand(async _ => await LoadSalePricesAsync());
        OpenAddDialogCommand = new RelayCommand(_ => OpenAddDialog());
        OpenEditDialogCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedItem != null);
        SaveItemCommand = new AsyncRelayCommand(_ => SaveItemAsync(), _ => CanSaveItem());
        CancelDialogCommand = new RelayCommand(_ => CancelDialog());
        DeleteItemCommand = new AsyncRelayCommand(DeleteItemAsync);

        _ = LoadSalePricesAsync();
    }

    private async Task LoadSalePricesAsync()
    {
        try
        {
            IsLoading = true;
            
            // Données locales pour la démo
            var localPrices = new List<SalePrice>
            {
                new SalePrice { Id = 1, ProductName = "Burrito Black Woods", Category = "Plat principal", Price = 15.00m, Cost = 8.50m, Margin = 43.33m, Description = "Burrito signature avec viande, légumes frais et sauce maison" },
                new SalePrice { Id = 2, ProductName = "Tacos Supreme", Category = "Plat principal", Price = 12.00m, Cost = 6.80m, Margin = 43.33m, Description = "3 tacos garnis avec viande de choix" },
                new SalePrice { Id = 3, ProductName = "Quesadilla Fromage", Category = "Entrée", Price = 8.00m, Cost = 4.20m, Margin = 47.50m, Description = "Tortilla grillée au fromage fondant" },
                new SalePrice { Id = 4, ProductName = "Nachos Deluxe", Category = "Accompagnement", Price = 9.50m, Cost = 5.00m, Margin = 47.37m, Description = "Nachos avec guacamole, crème et jalapeños" },
                new SalePrice { Id = 5, ProductName = "Coca-Cola", Category = "Boisson", Price = 3.50m, Cost = 1.20m, Margin = 65.71m, Description = "Boisson gazeuse 33cl" },
                new SalePrice { Id = 6, ProductName = "Menu Burrito", Category = "Menu", Price = 18.50m, Cost = 10.70m, Margin = 42.16m, Description = "Burrito + accompagnement + boisson" }
            };
            
            // Appliquer les filtres
            if (!string.IsNullOrEmpty(SearchText))
                localPrices = localPrices.Where(p => p.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                                                    p.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(FilterCategory))
                localPrices = localPrices.Where(p => p.Category == FilterCategory).ToList();

            SalePrices = new ObservableCollection<SalePrice>(localPrices);
            Log.Information($"Loaded {localPrices.Count} sale prices");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading sale prices");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OpenAddDialog()
    {
        Log.Information("[VM] OpenAddDialog called for sale prices");
        _isEditing = false;
        NewProductName = string.Empty;
        NewCategory = "Plat principal";
        NewPrice = 0;
        NewCost = 0;
        NewDescription = string.Empty;
        IsAddEditDialogOpen = true;
        Log.Information("[VM] Dialog opened, IsAddEditDialogOpen={IsOpen}", IsAddEditDialogOpen);
    }

    private void OpenEditDialog()
    {
        if (SelectedItem == null) return;

        _isEditing = true;
        _editingId = SelectedItem.Id;
        NewProductName = SelectedItem.ProductName;
        NewCategory = SelectedItem.Category;
        NewPrice = SelectedItem.Price;
        NewCost = SelectedItem.Cost;
        NewDescription = SelectedItem.Description;
        IsAddEditDialogOpen = true;
    }

    private bool CanSaveItem()
    {
        return !string.IsNullOrWhiteSpace(NewProductName) && 
               NewPrice > 0 && NewCost >= 0;
    }

    private async Task SaveItemAsync(object? parameter = null)
    {
        try
        {
            var item = new SalePrice
            {
                ProductName = NewProductName,
                Category = NewCategory,
                Price = NewPrice,
                Cost = NewCost,
                Margin = NewMargin,
                Description = NewDescription,
                IsActive = true,
                UpdatedAt = DateTime.Now
            };

            if (_isEditing)
            {
                item.Id = _editingId;
                Log.Information($"Sale price updated: {item.ProductName}");
            }
            else
            {
                item.CreatedAt = DateTime.Now;
                Log.Information($"Sale price created: {item.ProductName}");
            }

            await LoadSalePricesAsync();
            CancelDialog();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving sale price");
        }
    }

    private async Task DeleteItemAsync(object? parameter)
    {
        if (parameter is SalePrice item)
        {
            try
            {
                Log.Information($"Sale price '{item.ProductName}' deleted");
                await LoadSalePricesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting sale price");
            }
        }
    }

    private void CancelDialog()
    {
        IsAddEditDialogOpen = false;
    }

    private void UpdateMargin()
    {
        OnPropertyChanged(nameof(NewMargin));
    }
}
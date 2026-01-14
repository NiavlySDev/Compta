using System.Collections.ObjectModel;
using System.Windows.Input;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class PurchasePricesViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    
    private ObservableCollection<PurchasePrice> _purchasePrices = new();
    private PurchasePrice? _selectedItem;
    private string _searchText = string.Empty;
    private string? _filterCategory;
    private string? _filterSupplier;
    private bool _isLoading;
    private bool _isAddEditDialogOpen;
    
    // New/Edit item properties
    private string _newProductName = string.Empty;
    private string _newCategory = "Matière première";
    private decimal _newUnitPrice;
    private string _newSupplier = string.Empty;
    
    private bool _isEditing;
    private int _editingId;

    public ObservableCollection<PurchasePrice> PurchasePrices
    {
        get => _purchasePrices;
        set => SetProperty(ref _purchasePrices, value);
    }

    public PurchasePrice? SelectedItem
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
                _ = LoadPurchasePricesAsync();
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
                _ = LoadPurchasePricesAsync();
            }
        }
    }

    public string? FilterSupplier
    {
        get => _filterSupplier;
        set
        {
            if (SetProperty(ref _filterSupplier, value))
            {
                _ = LoadPurchasePricesAsync();
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

    public decimal NewUnitPrice
    {
        get => _newUnitPrice;
        set => SetProperty(ref _newUnitPrice, value);
    }

    public string NewSupplier
    {
        get => _newSupplier;
        set => SetProperty(ref _newSupplier, value);
    }

    public AsyncRelayCommand LoadPurchasePricesCommand { get; }
    public ICommand OpenAddDialogCommand { get; }
    public ICommand OpenEditDialogCommand { get; }
    public AsyncRelayCommand SaveItemCommand { get; }
    public ICommand CancelDialogCommand { get; }
    public AsyncRelayCommand DeleteItemCommand { get; }

    public List<string> Categories { get; } = new()
    {
        "Matière première", "Légumes", "Viandes", "Produits Laitiers", "Ingrédients", "Fruits"
    };

    public ObservableCollection<Supplier> Suppliers { get; } = new();

    public PurchasePricesViewModel(IDataService dataService, IAuthService authService)
    {
        _dataService = dataService;
        _authService = authService;

        LoadPurchasePricesCommand = new AsyncRelayCommand(async _ => await LoadPurchasePricesAsync());
        OpenAddDialogCommand = new RelayCommand(_ => OpenAddDialog());
        OpenEditDialogCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedItem != null);
        SaveItemCommand = new AsyncRelayCommand(_ => SaveItemAsync(), _ => CanSaveItem());
        CancelDialogCommand = new RelayCommand(_ => CancelDialog());
        DeleteItemCommand = new AsyncRelayCommand(DeleteItemAsync);

        _ = LoadPurchasePricesAsync();
        _ = LoadSuppliersAsync();
    }

    private async Task LoadSuppliersAsync()
    {
        try
        {
            var suppliers = await _dataService.GetSuppliersAsync();
            Suppliers.Clear();
            foreach (var supplier in suppliers)
            {
                Suppliers.Add(supplier);
            }
            Log.Information("[VM] Loaded {Count} suppliers for purchase prices", Suppliers.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading suppliers for purchase prices");
        }
    }

    private async Task LoadPurchasePricesAsync()
    {
        try
        {
            IsLoading = true;
            
            // Données locales pour la démo
            var localPrices = new List<PurchasePrice>
            {
                new PurchasePrice { Id = 1, ProductName = "Steak de bœuf", Category = "Viandes", UnitPrice = 90.00m, Supplier = "Boucherie Jackland", LastUpdated = DateTime.Now.AddDays(-2) },
                new PurchasePrice { Id = 2, ProductName = "Farine de blé", Category = "Ingrédients", UnitPrice = 80.00m, Supplier = "Molienda Hermandad", LastUpdated = DateTime.Now.AddDays(-5) },
                new PurchasePrice { Id = 3, ProductName = "Oignon", Category = "Légumes", UnitPrice = 10.00m, Supplier = "Woods Farm", LastUpdated = DateTime.Now.AddDays(-1) },
                new PurchasePrice { Id = 4, ProductName = "Tomate", Category = "Légumes", UnitPrice = 20.00m, Supplier = "Theronis Harvest", LastUpdated = DateTime.Now.AddDays(-3) },
                new PurchasePrice { Id = 5, ProductName = "Lait", Category = "Produits Laitiers", UnitPrice = 20.00m, Supplier = "Woods Farm", LastUpdated = DateTime.Now.AddDays(-1) }
            };
            
            // Appliquer les filtres
            if (!string.IsNullOrEmpty(SearchText))
                localPrices = localPrices.Where(p => p.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                                                    p.Supplier.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (!string.IsNullOrEmpty(FilterCategory))
                localPrices = localPrices.Where(p => p.Category == FilterCategory).ToList();
            
            if (!string.IsNullOrEmpty(FilterSupplier))
                localPrices = localPrices.Where(p => p.Supplier == FilterSupplier).ToList();

            PurchasePrices = new ObservableCollection<PurchasePrice>(localPrices);
            Log.Information($"Loaded {localPrices.Count} purchase prices");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading purchase prices");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OpenAddDialog()
    {
        Log.Information("[VM] OpenAddDialog called, Suppliers count={Count}", Suppliers.Count);
        _isEditing = false;
        NewProductName = string.Empty;
        NewCategory = "Matière première";
        NewUnitPrice = 0;
        NewSupplier = string.Empty;
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
        NewUnitPrice = SelectedItem.UnitPrice;
        NewSupplier = SelectedItem.Supplier;
        IsAddEditDialogOpen = true;
    }

    private bool CanSaveItem()
    {
        return !string.IsNullOrWhiteSpace(NewProductName) && 
               !string.IsNullOrWhiteSpace(NewSupplier) && 
               NewUnitPrice > 0;
    }

    private async Task SaveItemAsync(object? parameter = null)
    {
        try
        {
            var item = new PurchasePrice
            {
                ProductName = NewProductName,
                Category = NewCategory,
                UnitPrice = NewUnitPrice,
                Supplier = NewSupplier,
                LastUpdated = DateTime.Now,
                IsActive = true,
                UpdatedAt = DateTime.Now
            };

            if (_isEditing)
            {
                item.Id = _editingId;
                Log.Information($"Purchase price updated: {item.ProductName}");
            }
            else
            {
                item.CreatedAt = DateTime.Now;
                Log.Information($"Purchase price created: {item.ProductName}");
            }

            await LoadPurchasePricesAsync();
            CancelDialog();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving purchase price");
        }
    }

    private async Task DeleteItemAsync(object? parameter)
    {
        if (parameter is PurchasePrice item)
        {
            try
            {
                Log.Information($"Purchase price '{item.ProductName}' deleted");
                await LoadPurchasePricesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting purchase price");
            }
        }
    }

    private void CancelDialog()
    {
        IsAddEditDialogOpen = false;
    }
}
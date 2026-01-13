using System.Collections.ObjectModel;
using System.Windows.Input;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class InventoryViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    
    private ObservableCollection<InventoryItem> _inventoryItems = new();
    private InventoryItem? _selectedItem;
    private string _searchText = string.Empty;
    private string? _filterCategory;
    private bool _showLowStockOnly;
    private bool _showExpiredOnly;
    private bool _isLoading;
    private bool _isAddEditDialogOpen;
    
    // New/Edit item properties
    private string _newProductName = string.Empty;
    private string _newCategory = "Matière première";
    private decimal _newQuantity;
    private string _newUnit = "kg";
    private decimal _newUnitCost;
    private decimal _newMinQuantity = 5;
    private string _newSupplier = string.Empty;
    private DateTime? _newExpiryDate;
    private bool _isEditing;
    private int _editingId;

    public ObservableCollection<InventoryItem> InventoryItems
    {
        get => _inventoryItems;
        set => SetProperty(ref _inventoryItems, value);
    }

    public InventoryItem? SelectedItem
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
                _ = LoadInventoryAsync();
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
                _ = LoadInventoryAsync();
            }
        }
    }

    public bool ShowLowStockOnly
    {
        get => _showLowStockOnly;
        set
        {
            if (SetProperty(ref _showLowStockOnly, value))
            {
                _ = LoadInventoryAsync();
            }
        }
    }

    public bool ShowExpiredOnly
    {
        get => _showExpiredOnly;
        set
        {
            if (SetProperty(ref _showExpiredOnly, value))
            {
                _ = LoadInventoryAsync();
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
        set
        {
            if (SetProperty(ref _newProductName, value))
            {
                (SaveItemCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string NewCategory
    {
        get => _newCategory;
        set
        {
            if (SetProperty(ref _newCategory, value))
            {
                // Si c'est un plat préparé, définir automatiquement la date d'expiration à 1 semaine
                if (value == "Plat préparé")
                {
                    NewExpiryDate = DateTime.Now.AddDays(7);
                }
                else
                {
                    NewExpiryDate = null;
                }
            }
        }
    }

    public decimal NewQuantity
    {
        get => _newQuantity;
        set => SetProperty(ref _newQuantity, value);
    }

    public string NewUnit
    {
        get => _newUnit;
        set => SetProperty(ref _newUnit, value);
    }

    public decimal NewUnitCost
    {
        get => _newUnitCost;
        set => SetProperty(ref _newUnitCost, value);
    }

    public decimal NewMinQuantity
    {
        get => _newMinQuantity;
        set => SetProperty(ref _newMinQuantity, value);
    }

    public string NewSupplier
    {
        get => _newSupplier;
        set => SetProperty(ref _newSupplier, value);
    }

    public DateTime? NewExpiryDate
    {
        get => _newExpiryDate;
        set => SetProperty(ref _newExpiryDate, value);
    }

    public List<string> Categories { get; } = new() { "Matière première", "Plat préparé" };
    public List<string> Units { get; } = new() { "kg", "g", "L", "mL", "pièce", "portion" };

    public ICommand LoadInventoryCommand { get; }
    public ICommand OpenAddDialogCommand { get; }
    public ICommand OpenEditDialogCommand { get; }
    public AsyncRelayCommand SaveItemCommand { get; }
    public ICommand CancelDialogCommand { get; }
    public AsyncRelayCommand DeleteItemCommand { get; }
    public AsyncRelayCommand AdjustStockCommand { get; }

    public InventoryViewModel(IDataService dataService, IAuthService authService)
    {
        _dataService = dataService;
        _authService = authService;

        LoadInventoryCommand = new AsyncRelayCommand(async _ => await LoadInventoryAsync());
        OpenAddDialogCommand = new RelayCommand(_ => OpenAddDialog());
        OpenEditDialogCommand = new RelayCommand(_ => OpenEditDialog());
        SaveItemCommand = new AsyncRelayCommand(_ => SaveItemAsync(), _ => CanSaveItem());
        CancelDialogCommand = new RelayCommand(_ => CancelDialog());
        DeleteItemCommand = new AsyncRelayCommand(DeleteItemAsync);
        AdjustStockCommand = new AsyncRelayCommand(AdjustStockAsync);

        _ = LoadInventoryAsync();
    }

    private async Task LoadInventoryAsync()
    {
        try
        {
            IsLoading = true;
            
            // FORCER l'utilisation des données locales pour éviter le bug System.Windows.Controls
            LoadLocalInventoryData();
            Log.Information($"Loaded {InventoryItems.Count} inventory items from local data");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading inventory items");
            LoadLocalInventoryData();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadLocalInventoryData()
    {
        var localItems = new List<InventoryItem>
        {
            new InventoryItem { Id = 1, ProductName = "Steak de bœuf", Category = "Viandes", Quantity = 497, Unit = "unités", UnitCost = 90.00m, MinQuantity = 50, Supplier = "Boucherie Jackland", ExpiryDate = DateTime.Now.AddDays(7) },
            new InventoryItem { Id = 2, ProductName = "Farine de blé", Category = "Ingrédients", Quantity = 498, Unit = "unités", UnitCost = 80.00m, MinQuantity = 100, Supplier = "Molienda Hermandad", ExpiryDate = DateTime.Now.AddMonths(6) },
            new InventoryItem { Id = 3, ProductName = "Huile", Category = "Ingrédients", Quantity = 575, Unit = "unités", UnitCost = 80.00m, MinQuantity = 50, Supplier = "Molienda Hermandad", ExpiryDate = DateTime.Now.AddYears(1) },
            new InventoryItem { Id = 4, ProductName = "Oignon", Category = "Légumes", Quantity = 532, Unit = "unités", UnitCost = 10.00m, MinQuantity = 50, Supplier = "Woods Farm", ExpiryDate = DateTime.Now.AddDays(14) },
            new InventoryItem { Id = 5, ProductName = "Tomate", Category = "Légumes", Quantity = 451, Unit = "unités", UnitCost = 20.00m, MinQuantity = 50, Supplier = "Theronis Harvest", ExpiryDate = DateTime.Now.AddDays(7) },
            new InventoryItem { Id = 6, ProductName = "Lait", Category = "Produits Laitiers", Quantity = 512, Unit = "unités", UnitCost = 20.00m, MinQuantity = 100, Supplier = "Woods Farm", ExpiryDate = DateTime.Now.AddDays(7) },
            new InventoryItem { Id = 7, ProductName = "Tortillas", Category = "Produits Finis", Quantity = 95, Unit = "unités", UnitCost = 0.00m, MinQuantity = 50, Supplier = "Black Woods", ExpiryDate = DateTime.Now.AddDays(7) }
        };
        
        // Appliquer les filtres
        if (!string.IsNullOrEmpty(SearchText))
            localItems = localItems.Where(i => i.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                                             i.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
        
        if (!string.IsNullOrEmpty(FilterCategory))
            localItems = localItems.Where(i => i.Category == FilterCategory).ToList();
        
        if (ShowLowStockOnly)
            localItems = localItems.Where(i => i.IsLowStock).ToList();
        
        if (ShowExpiredOnly)
            localItems = localItems.Where(i => i.IsExpired || i.IsExpiringSoon).ToList();

        InventoryItems = new ObservableCollection<InventoryItem>(localItems);
    }

    private void OpenAddDialog()
    {
        _isEditing = false;
        NewProductName = string.Empty;
        NewCategory = "Matière première";
        NewQuantity = 0;
        NewUnit = "kg";
        NewUnitCost = 0;
        NewMinQuantity = 5;
        NewSupplier = string.Empty;
        NewExpiryDate = null;
        IsAddEditDialogOpen = true;
    }

    private void OpenEditDialog()
    {
        if (SelectedItem == null) return;

        _isEditing = true;
        _editingId = SelectedItem.Id;
        NewProductName = SelectedItem.ProductName;
        NewCategory = SelectedItem.Category;
        NewQuantity = SelectedItem.Quantity;
        NewUnit = SelectedItem.Unit;
        NewUnitCost = SelectedItem.UnitCost;
        NewMinQuantity = SelectedItem.MinQuantity;
        NewSupplier = SelectedItem.Supplier ?? string.Empty;
        NewExpiryDate = SelectedItem.ExpiryDate;
        IsAddEditDialogOpen = true;
    }

    private bool CanSaveItem()
    {
        return !string.IsNullOrWhiteSpace(NewProductName) && NewQuantity >= 0 && NewUnitCost >= 0;
    }

    private async Task SaveItemAsync(object? parameter = null)
    {
        try
        {
            var item = new InventoryItem
            {
                ProductName = NewProductName,
                Category = NewCategory,
                Quantity = NewQuantity,
                Unit = NewUnit,
                UnitCost = NewUnitCost,
                MinQuantity = NewMinQuantity,
                Supplier = string.IsNullOrWhiteSpace(NewSupplier) ? null : NewSupplier,
                ExpiryDate = NewExpiryDate,
                UpdatedAt = DateTime.Now
            };

            if (_isEditing)
            {
                item.Id = _editingId;
                var success = await _dataService.UpdateInventoryItemAsync(item);
                Log.Information(success ? "Inventory item updated successfully" : "Failed to update inventory item");
            }
            else
            {
                item.CreatedAt = DateTime.Now;
                var created = await _dataService.CreateInventoryItemAsync(item);
                Log.Information(created != null ? "Inventory item created successfully" : "Failed to create inventory item");
            }

            await LoadInventoryAsync();
            CancelDialog();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving inventory item");
        }
    }

    private async Task DeleteItemAsync(object? parameter)
    {
        if (parameter is InventoryItem item)
        {
            try
            {
                var success = await _dataService.DeleteInventoryItemAsync(item.Id);
                if (success)
                {
                    await LoadInventoryAsync();
                    Log.Information($"Inventory item '{item.ProductName}' deleted");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting inventory item");
            }
        }
    }

    private Task AdjustStockAsync(object? parameter)
    {
        // TODO: Implement stock adjustment dialog
        Log.Information("Stock adjustment not yet implemented");
        return Task.CompletedTask;
    }

    private void CancelDialog()
    {
        IsAddEditDialogOpen = false;
    }
}
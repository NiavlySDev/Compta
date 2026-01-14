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

    public ObservableCollection<Supplier> Suppliers { get; } = new();

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
        LoadSuppliersAsync();
    }

    private async Task LoadInventoryAsync()
    {
        try
        {
            IsLoading = true;
            
            // Charger depuis la base de données
            var allItems = await _dataService.GetInventoryAsync();
            var filteredItems = allItems.AsEnumerable();
            
            // Appliquer les filtres
            if (!string.IsNullOrEmpty(SearchText))
                filteredItems = filteredItems.Where(i => i.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                                                 i.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrEmpty(FilterCategory))
                filteredItems = filteredItems.Where(i => i.Category == FilterCategory);
            
            if (ShowLowStockOnly)
                filteredItems = filteredItems.Where(i => i.IsLowStock);
            
            if (ShowExpiredOnly)
                filteredItems = filteredItems.Where(i => i.IsExpired || i.IsExpiringSoon);

            InventoryItems = new ObservableCollection<InventoryItem>(filteredItems);
            Log.Information($"Loaded {InventoryItems.Count} inventory items from database");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading inventory items");
            InventoryItems = new ObservableCollection<InventoryItem>();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private Task LoadSuppliersAsync()
    {
        try
        {
            // Pour l'instant, on va charger des fournisseurs statiques
            // En attendant d'avoir GetSuppliersAsync dans l'API
            Suppliers.Clear();
            
            var mockSuppliers = new List<Supplier>
            {
                new() { Id = 1, Name = "Boucherie Jackland" },
                new() { Id = 2, Name = "Molienda Hermandad" },
                new() { Id = 3, Name = "Woods Farm" },
                new() { Id = 4, Name = "Theronis Harvest" },
                new() { Id = 5, Name = "Black Woods" }
            };
            
            foreach (var supplier in mockSuppliers)
            {
                Suppliers.Add(supplier);
            }
            Log.Information($"Loaded {mockSuppliers.Count} suppliers");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading suppliers");
            return Task.CompletedTask;
        }
    }

    private void OpenAddDialog()
    {
        _isEditing = false;
        NewProductName = string.Empty;
        NewCategory = "Matière première";
        NewQuantity = 0;
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
        NewMinQuantity = SelectedItem.MinQuantity;
        NewSupplier = SelectedItem.Supplier ?? string.Empty;
        NewExpiryDate = SelectedItem.ExpiryDate;
        IsAddEditDialogOpen = true;
    }

    private bool CanSaveItem()
    {
        return !string.IsNullOrWhiteSpace(NewProductName) && NewQuantity >= 0;
    }

    private async Task SaveItemAsync(object? parameter = null)
    {
        try
        {
            Log.Information("[VM] SaveItemAsync called: IsEditing={IsEditing}, ProductName={ProductName}, Category={Category}", 
                _isEditing, NewProductName, NewCategory);
            
            var item = new InventoryItem
            {
                ProductName = NewProductName,
                Category = NewCategory,
                Quantity = NewQuantity,
                MinQuantity = NewMinQuantity,
                Supplier = string.IsNullOrWhiteSpace(NewSupplier) ? null : NewSupplier,
                ExpiryDate = NewExpiryDate,
                UpdatedAt = DateTime.Now
            };

            if (_isEditing)
            {
                item.Id = _editingId;
                var success = await _dataService.UpdateInventoryItemAsync(item);
                if (success)
                {
                    Log.Information("Inventory item updated successfully");
                    await LoadInventoryAsync();
                }
                else
                {
                    Log.Warning("Failed to update inventory item");
                }
            }
            else
            {
                item.CreatedAt = DateTime.Now;
                var created = await _dataService.CreateInventoryItemAsync(item);
                if (created != null)
                {
                    Log.Information("Inventory item created successfully");
                    await LoadInventoryAsync();
                }
                else
                {
                    Log.Warning("Failed to create inventory item");
                }
            }

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
                    InventoryItems.Remove(item);
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
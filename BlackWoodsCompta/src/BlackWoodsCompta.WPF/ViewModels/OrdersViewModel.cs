using System.Collections.ObjectModel;
using System.Windows.Input;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.Models.Enums;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class OrdersViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    
    private ObservableCollection<Order> _orders = new();
    private Order? _selectedOrder;
    private ObservableCollection<OrderItem> _orderItems = new();
    private ObservableCollection<Supplier> _suppliers = new();
    private string _searchText = string.Empty;
    private string? _filterStatus;
    private bool _isLoading;
    private bool _isAddEditDialogOpen;
    
    // New/Edit order properties
    private int? _selectedSupplierId;
    private DateTime? _expectedDeliveryDate = DateTime.Now.AddDays(7);
    private string _orderNotes = string.Empty;
    
    // New item properties for current order
    private string _productName = string.Empty;
    private decimal _quantity;
    private decimal _unitPrice;
    
    private string _newOrderNumber = string.Empty;
    private string _newSupplier = string.Empty;
    private DateTime _newOrderDate = DateTime.Now;
    private DateTime? _newDeliveryDate;
    private string _newStatus = "En attente";
    private string _newNotes = string.Empty;
    
    // New item properties
    private string _newItemProductName = string.Empty;
    private string _newItemCategory = "Matière première";
    private decimal _newItemQuantity;
    private decimal _newItemUnitPrice;
    private DateTime? _newItemExpiryDate;
    
    private bool _isEditing;
    private int _editingId;

    public ObservableCollection<Order> Orders
    {
        get => _orders;
        set => SetProperty(ref _orders, value);
    }

    public Order? SelectedOrder
    {
        get => _selectedOrder;
        set => SetProperty(ref _selectedOrder, value);
    }

    public ObservableCollection<OrderItem> OrderItems
    {
        get => _orderItems;
        set
        {
            if (SetProperty(ref _orderItems, value))
            {
                OnPropertyChanged(nameof(CurrentOrderItems));
                OnPropertyChanged(nameof(HasOrderItems));
                OnPropertyChanged(nameof(TotalAmount));
            }
        }
    }

    public ObservableCollection<OrderItem> CurrentOrderItems => OrderItems;
    
    public bool HasOrderItems => OrderItems.Any();

    public ObservableCollection<Supplier> Suppliers
    {
        get => _suppliers;
        set => SetProperty(ref _suppliers, value);
    }

    public int? SelectedSupplierId
    {
        get => _selectedSupplierId;
        set => SetProperty(ref _selectedSupplierId, value);
    }

    public DateTime? ExpectedDeliveryDate
    {
        get => _expectedDeliveryDate;
        set => SetProperty(ref _expectedDeliveryDate, value);
    }

    public string OrderNotes
    {
        get => _orderNotes;
        set => SetProperty(ref _orderNotes, value);
    }

    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value);
    }

    public decimal Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set => SetProperty(ref _unitPrice, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _ = LoadOrdersAsync();
            }
        }
    }

    public string? FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
            {
                _ = LoadOrdersAsync();
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

    public string NewOrderNumber
    {
        get => _newOrderNumber;
        set
        {
            if (SetProperty(ref _newOrderNumber, value))
            {
                (SaveOrderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string NewSupplier
    {
        get => _newSupplier;
        set
        {
            if (SetProperty(ref _newSupplier, value))
            {
                (SaveOrderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public DateTime NewOrderDate
    {
        get => _newOrderDate;
        set => SetProperty(ref _newOrderDate, value);
    }

    public DateTime? NewDeliveryDate
    {
        get => _newDeliveryDate;
        set => SetProperty(ref _newDeliveryDate, value);
    }

    public string NewStatus
    {
        get => _newStatus;
        set => SetProperty(ref _newStatus, value);
    }

    public string NewNotes
    {
        get => _newNotes;
        set => SetProperty(ref _newNotes, value);
    }

    public string NewItemProductName
    {
        get => _newItemProductName;
        set => SetProperty(ref _newItemProductName, value);
    }

    public string NewItemCategory
    {
        get => _newItemCategory;
        set
        {
            if (SetProperty(ref _newItemCategory, value))
            {
                if (value == "Plat préparé")
                {
                    NewItemExpiryDate = DateTime.Now.AddDays(7);
                }
                else
                {
                    NewItemExpiryDate = null;
                }
            }
        }
    }

    public decimal NewItemQuantity
    {
        get => _newItemQuantity;
        set => SetProperty(ref _newItemQuantity, value);
    }

    public decimal NewItemUnitPrice
    {
        get => _newItemUnitPrice;
        set => SetProperty(ref _newItemUnitPrice, value);
    }

    public DateTime? NewItemExpiryDate
    {
        get => _newItemExpiryDate;
        set => SetProperty(ref _newItemExpiryDate, value);
    }

    public decimal TotalAmount => OrderItems.Sum(i => i.TotalPrice);

    public List<string> OrderStatuses { get; } = new() { "En attente", "Livrée", "Annulée" };
    public List<string> Categories { get; } = new() { "Matière première", "Plat préparé" };
    public List<string> Units { get; } = new() { "kg", "g", "L", "mL", "pièce", "portion" };

    public ICommand LoadOrdersCommand { get; }
    public ICommand OpenAddDialogCommand { get; }
    public ICommand OpenEditDialogCommand { get; }
    public AsyncRelayCommand SaveOrderCommand { get; }
    public ICommand CancelDialogCommand { get; }
    public AsyncRelayCommand DeleteOrderCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand AddOrderItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public AsyncRelayCommand DeliverOrderCommand { get; }

    public OrdersViewModel(IDataService dataService, IAuthService authService)
    {
        _dataService = dataService;
        _authService = authService;

        LoadOrdersCommand = new AsyncRelayCommand(_ => LoadOrdersAsync());
        OpenAddDialogCommand = new RelayCommand(_ => OpenAddDialog());
        OpenEditDialogCommand = new RelayCommand(_ => OpenEditDialog());
        SaveOrderCommand = new AsyncRelayCommand(_ => SaveOrderAsync(), _ => CanSaveOrder());
        CancelDialogCommand = new RelayCommand(_ => CancelDialog());
        DeleteOrderCommand = new AsyncRelayCommand(DeleteOrderAsync);
        AddItemCommand = new RelayCommand(_ => AddItem());
        AddOrderItemCommand = new RelayCommand(_ => AddOrderItem());
        RemoveItemCommand = new RelayCommand(RemoveItem);
        DeliverOrderCommand = new AsyncRelayCommand(DeliverOrderAsync);

        _ = LoadOrdersAsync();
        _ = LoadSuppliersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        try
        {
            IsLoading = true;
            var orders = await _dataService.GetOrdersAsync(SearchText, FilterStatus);
            Orders = new ObservableCollection<Order>(orders);
            Log.Information($"Loaded {orders.Count} orders");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading orders");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OpenAddDialog()
    {
        _isEditing = false;
        NewOrderNumber = GenerateOrderNumber();
        NewSupplier = string.Empty;
        NewOrderDate = DateTime.Now;
        NewDeliveryDate = null;
        NewStatus = "En attente";
        NewNotes = string.Empty;
        OrderItems.Clear();
        IsAddEditDialogOpen = true;
    }

    private void OpenEditDialog()
    {
        if (SelectedOrder == null) return;

        _isEditing = true;
        _editingId = SelectedOrder.Id;
        NewOrderNumber = SelectedOrder.OrderNumber;
        NewSupplier = SelectedOrder.Supplier;
        NewOrderDate = SelectedOrder.OrderDate;
        NewDeliveryDate = SelectedOrder.DeliveryDate;
        NewStatus = SelectedOrder.Status;
        NewNotes = SelectedOrder.Notes ?? string.Empty;
        OrderItems = new ObservableCollection<OrderItem>(SelectedOrder.Items);
        IsAddEditDialogOpen = true;
    }

    private string GenerateOrderNumber()
    {
        return $"CMD-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
    }

    private bool CanSaveOrder()
    {
        return SelectedSupplierId.HasValue && OrderItems.Any();
    }

    private async Task SaveOrderAsync(object? parameter = null)
    {
        try
        {
            // Générer numéro de commande automatique si absent
            var orderNumber = string.IsNullOrWhiteSpace(NewOrderNumber) 
                ? $"CMD-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Ticks % 1000:000}" 
                : NewOrderNumber;
            
            // Récupérer nom fournisseur
            var supplier = Suppliers.FirstOrDefault(s => s.Id == SelectedSupplierId);
            if (supplier == null)
            {
                Log.Error("[VM] Supplier not found for ID={SupplierId}", SelectedSupplierId);
                return;
            }

            var order = new Order
            {
                OrderNumber = orderNumber,
                Supplier = supplier.Name,
                OrderDate = NewOrderDate,
                DeliveryDate = ExpectedDeliveryDate,
                Status = NewStatus,
                TotalAmount = TotalAmount,
                Notes = string.IsNullOrWhiteSpace(OrderNotes) ? null : OrderNotes,
                UserId = _authService.CurrentUser?.Id ?? 1,
                Items = OrderItems.ToList()
            };

            if (_isEditing)
            {
                order.Id = _editingId;
                var success = await _dataService.UpdateOrderAsync(order);
                Log.Information(success ? "Order updated successfully" : "Failed to update order");
            }
            else
            {
                var created = await _dataService.CreateOrderAsync(order);
                if (created != null)
                {
                    // Créer la transaction de dépense
                    var transaction = new Transaction
                    {
                        Type = TransactionType.Depense,
                        Category = "Approvisionnement",
                        Amount = order.TotalAmount,
                        Description = $"Commande {order.OrderNumber} - {order.Supplier}",
                        Reference = order.OrderNumber,
                        UserId = order.UserId,
                        CreatedAt = order.OrderDate
                    };

                    var transactionResult = await _dataService.CreateTransactionAsync(transaction);
                    if (transactionResult != null)
                    {
                        // Lier la transaction à la commande
                        created.TransactionId = transactionResult.Id;
                        await _dataService.UpdateOrderAsync(created);
                        Log.Information($"Order created successfully with transaction {transactionResult.Id}");
                    }
                    else
                    {
                        Log.Information("Order created successfully");
                    }
                }
            }

            await LoadOrdersAsync();
            CancelDialog();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving order");
        }
    }

    private async Task DeleteOrderAsync(object? parameter)
    {
        if (parameter is Order order)
        {
            try
            {
                // Si la commande a une transaction liée, la supprimer aussi
                if (order.TransactionId.HasValue)
                {
                    await _dataService.DeleteTransactionAsync(order.TransactionId.Value);
                }

                var success = await _dataService.DeleteOrderAsync(order.Id);
                if (success)
                {
                    await LoadOrdersAsync();
                    Log.Information($"Order '{order.OrderNumber}' deleted");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting order");
            }
        }
    }

    private void AddItem()
    {
        Log.Information("[VM] AddItem called: ProductName={ProductName}, Quantity={Quantity}, UnitPrice={UnitPrice}", 
            NewItemProductName, NewItemQuantity, NewItemUnitPrice);
            
        if (string.IsNullOrWhiteSpace(NewItemProductName) || NewItemQuantity <= 0 || NewItemUnitPrice <= 0)
        {
            Log.Warning("[VM] AddItem validation failed");
            return;
        }

        var item = new OrderItem
        {
            ProductName = NewItemProductName,
            Category = NewItemCategory,
            Quantity = NewItemQuantity,
            UnitPrice = NewItemUnitPrice,
            TotalPrice = NewItemQuantity * NewItemUnitPrice,
            ExpiryDate = NewItemExpiryDate
        };

        OrderItems.Add(item);
        Log.Information("[VM] Item added to OrderItems, count={Count}", OrderItems.Count);

        // Reset form
        NewItemProductName = string.Empty;
        NewItemQuantity = 0;
        NewItemUnitPrice = 0;
        NewItemExpiryDate = NewItemCategory == "Plat préparé" ? DateTime.Now.AddDays(7) : null;

        OnPropertyChanged(nameof(CurrentOrderItems));
        OnPropertyChanged(nameof(HasOrderItems));
        OnPropertyChanged(nameof(TotalAmount));
        (SaveOrderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
    }

    private void RemoveItem(object? parameter)
    {
        if (parameter is OrderItem item)
        {
            OrderItems.Remove(item);
            OnPropertyChanged(nameof(CurrentOrderItems));
            OnPropertyChanged(nameof(HasOrderItems));
            OnPropertyChanged(nameof(TotalAmount));
            (SaveOrderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    private async Task DeliverOrderAsync(object? parameter)
    {
        if (parameter is Order order)
        {
            try
            {
                order.Status = "Livrée";
                order.DeliveryDate = DateTime.Now;
                var success = await _dataService.UpdateOrderAsync(order);
                
                if (success)
                {
                    // Ajouter les articles à l'inventaire
                    foreach (var item in order.Items)
                    {
                        var inventoryItem = new InventoryItem
                        {
                            ProductName = item.ProductName,
                            Category = item.Category,
                            Quantity = item.Quantity,
                            MinQuantity = 5, // Valeur par défaut
                            Supplier = order.Supplier,
                            ExpiryDate = item.ExpiryDate,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        await _dataService.CreateInventoryItemAsync(inventoryItem);
                    }

                    await LoadOrdersAsync();
                    Log.Information($"Order '{order.OrderNumber}' marked as delivered and added to inventory");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error delivering order");
            }
        }
    }

    private void CancelDialog()
    {
        IsAddEditDialogOpen = false;
        OrderItems.Clear();
    }

    private async Task LoadSuppliersAsync()
    {
        try
        {
            var suppliers = await _dataService.GetSuppliersAsync();
            Suppliers = new ObservableCollection<Supplier>(suppliers);
            Log.Information($"Loaded {suppliers.Count} suppliers");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading suppliers");
        }
    }

    private void AddOrderItem()
    {
        Log.Information("[VM] AddOrderItem called: ProductName={ProductName}, Quantity={Quantity}, UnitPrice={UnitPrice}", 
            ProductName, Quantity, UnitPrice);
            
        if (string.IsNullOrWhiteSpace(ProductName) || Quantity <= 0 || UnitPrice <= 0)
        {
            Log.Warning("[VM] AddOrderItem validation failed");
            return;
        }

        var item = new OrderItem
        {
            ProductName = ProductName,
            Category = "Matière première",
            Quantity = Quantity,
            UnitPrice = UnitPrice,
            TotalPrice = Quantity * UnitPrice
        };

        OrderItems.Add(item);
        Log.Information("[VM] Item added to OrderItems, count={Count}", OrderItems.Count);

        // Reset form
        ProductName = string.Empty;
        Quantity = 0;
        UnitPrice = 0;

        OnPropertyChanged(nameof(CurrentOrderItems));
        OnPropertyChanged(nameof(HasOrderItems));
        OnPropertyChanged(nameof(TotalAmount));
        (SaveOrderCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
    }
}
using System.Collections.ObjectModel;
using System.Windows.Input;
using BlackWoodsCompta.Models.DTOs;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.Models.Enums;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class TransactionsViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    private bool _isLoading;
    private ObservableCollection<Transaction> _transactions = new();
    private Transaction? _selectedTransaction;
    private bool _isAddEditDialogOpen;
    private TransactionType _newType = TransactionType.Vente;
    private string _newCategory = string.Empty;
    private decimal _newAmount;
    private string _newDescription = string.Empty;
    private string _searchText = string.Empty;
    private DateTime _newDate = DateTime.Now;
    private int? _newEmployeeId;
    private bool _isEditing;
    private int _editingId;
    private ObservableCollection<Employee> _employees = new();

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<Transaction> Transactions
    {
        get => _transactions;
        set => SetProperty(ref _transactions, value);
    }

    public Transaction? SelectedTransaction
    {
        get => _selectedTransaction;
        set => SetProperty(ref _selectedTransaction, value);
    }

    public bool IsAddEditDialogOpen
    {
        get => _isAddEditDialogOpen;
        set => SetProperty(ref _isAddEditDialogOpen, value);
    }

    public TransactionType NewType
    {
        get => _newType;
        set
        {
            if (SetProperty(ref _newType, value))
            {
                (SaveTransactionCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
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
                (SaveTransactionCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public decimal NewAmount
    {
        get => _newAmount;
        set
        {
            if (SetProperty(ref _newAmount, value))
            {
                (SaveTransactionCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string NewDescription
    {
        get => _newDescription;
        set => SetProperty(ref _newDescription, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public DateTime NewDate
    {
        get => _newDate;
        set => SetProperty(ref _newDate, value);
    }

    public int? NewEmployeeId
    {
        get => _newEmployeeId;
        set => SetProperty(ref _newEmployeeId, value);
    }

    public ObservableCollection<Employee> Employees
    {
        get => _employees;
        set => SetProperty(ref _employees, value);
    }

    public Array TransactionTypes => Enum.GetValues(typeof(TransactionType));

    public List<string> Categories { get; } = new()
    {
        "Nourriture",
        "Boissons",
        "Salaires",
        "Fournitures",
        "Loyer",
        "Électricité",
        "Entretien",
        "Marketing",
        "Autre"
    };

    public ICommand LoadTransactionsCommand { get; }
    public ICommand OpenAddDialogCommand { get; }
    public ICommand OpenEditDialogCommand { get; }
    public ICommand SaveTransactionCommand { get; }
    public ICommand CancelDialogCommand { get; }
    public ICommand DeleteTransactionCommand { get; }
    public ICommand SearchCommand { get; }

    public TransactionsViewModel(IDataService dataService, IAuthService authService)
    {
        _dataService = dataService;
        _authService = authService;

        LoadTransactionsCommand = new AsyncRelayCommand(_ => LoadTransactionsAsync());
        OpenAddDialogCommand = new RelayCommand(_ => OpenAddDialog());
        OpenEditDialogCommand = new RelayCommand(OpenEditDialogWithParameter);
        SaveTransactionCommand = new AsyncRelayCommand(_ => SaveTransactionAsync(), _ => CanSaveTransaction());
        CancelDialogCommand = new RelayCommand(_ => CancelDialog());
        DeleteTransactionCommand = new AsyncRelayCommand(DeleteTransactionAsync);
        SearchCommand = new AsyncRelayCommand(_ => LoadTransactionsAsync());

        _ = LoadTransactionsAsync(null);
        _ = LoadEmployeesAsync();
    }

    private async Task LoadEmployeesAsync()
    {
        try
        {
            var employees = await _dataService.GetEmployeesAsync(null, null, true);
            Employees = new ObservableCollection<Employee>(employees);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading employees");
        }
    }

    private async Task LoadTransactionsAsync(object? parameter = null)
    {
        try
        {
            IsLoading = true;

            // Charger les transactions depuis la base de données
            var allTransactions = await _dataService.GetTransactionsAsync();
            
            // Si pas de transactions en DB, charger les données d'exemple
            if (!allTransactions.Any())
            {
                var mockTransactions = GetRealTransactionsData();
                // Sauvegarder les données d'exemple en DB
                foreach (var mockTransaction in mockTransactions)
                {
                    await _dataService.CreateTransactionAsync(mockTransaction);
                }
                allTransactions = await _dataService.GetTransactionsAsync();
            }

            var transactions = allTransactions.ToList();

            if (transactions != null)
            {
                // Filtrer selon le texte de recherche si nécessaire
                if (!string.IsNullOrEmpty(SearchText))
                {
                    transactions = transactions.Where(t => 
                        t.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        t.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        t.Amount.ToString().Contains(SearchText)
                    ).ToList();
                }

                Transactions = new ObservableCollection<Transaction>(transactions.OrderByDescending(t => t.CreatedAt));
                Log.Information($"Loaded {transactions.Count} transactions from database");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading transactions");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private List<Transaction> GetRealTransactionsData()
    {
        var transactions = new List<Transaction>();

        // === DÉPENSES RÉELLES (du fichier depenses.txt) ===
        
        // 11/01/2026 - Boucherie Jackland
        transactions.Add(new Transaction
        {
            Id = 1,
            Type = TransactionType.Depense,
            Category = "Achats Viandes",
            Amount = 27000.00m,
            Description = "Steak de boeuf - 300 unités à $90.00 - Boucherie Jackland - Récupéré par Jason Parker",
            CreatedAt = new DateTime(2026, 1, 11, 19, 0, 0),
            Reference = "DEP-2026-001"
        });

        // 12/01/2026 - Molienda Hermandad
        transactions.Add(new Transaction
        {
            Id = 2,
            Type = TransactionType.Depense,
            Category = "Achats Ingrédients",
            Amount = 4000.00m,
            Description = "Farine de maïs - 50 kg à $80.00 - Molienda Hermandad - Récupéré par Jason Parker",
            CreatedAt = new DateTime(2026, 1, 12, 17, 23, 33),
            Reference = "DEP-2026-002"
        });

        transactions.Add(new Transaction
        {
            Id = 3,
            Type = TransactionType.Depense,
            Category = "Achats Ingrédients",
            Amount = 16000.00m,
            Description = "Huile - 200 litres à $80.00 - Molienda Hermandad - Récupéré par Jason Parker",
            CreatedAt = new DateTime(2026, 1, 12, 17, 23, 33),
            Reference = "DEP-2026-003"
        });

        // 12/01/2026 - Woods Farm (À rembourser Anne Holmes)
        transactions.Add(new Transaction
        {
            Id = 4,
            Type = TransactionType.Depense,
            Category = "Oignon - 100 unités à $10.00",
            Amount = 1000.00m,
            Description = "Woods Farm - Récupéré par Anne Holmes - ⚠️ À REMBOURSER",
            CreatedAt = new DateTime(2026, 1, 12, 21, 0, 0),
            Reference = "DEP-2026-004"
        });

        transactions.Add(new Transaction
        {
            Id = 5,
            Type = TransactionType.Depense,
            Category = "Pommes de terre - 300 unités à $10.00",
            Amount = 3000.00m,
            Description = "Woods Farm - Récupéré par Anne Holmes - ⚠️ À REMBOURSER",
            CreatedAt = new DateTime(2026, 1, 12, 21, 0, 0),
            Reference = "DEP-2026-005"
        });

        transactions.Add(new Transaction
        {
            Id = 6,
            Type = TransactionType.Depense,
            Category = "Maïs - 200 unités à $10.00",
            Amount = 2000.00m,
            Description = "Woods Farm - Récupéré par Anne Holmes - ⚠️ À REMBOURSER",
            CreatedAt = new DateTime(2026, 1, 12, 21, 0, 0),
            Reference = "DEP-2026-006"
        });

        transactions.Add(new Transaction
        {
            Id = 7,
            Type = TransactionType.Depense,
            Category = "Lait - 300 unités à $20.00",
            Amount = 6000.00m,
            Description = "Woods Farm - Récupéré par Anne Holmes - ⚠️ À REMBOURSER",
            CreatedAt = new DateTime(2026, 1, 12, 21, 0, 0),
            Reference = "DEP-2026-007"
        });

        transactions.Add(new Transaction
        {
            Id = 8,
            Type = TransactionType.Depense,
            Category = "Oeuf - 400 unités à $10.00",
            Amount = 4000.00m,
            Description = "Woods Farm - Récupéré par Anne Holmes - ⚠️ À REMBOURSER",
            CreatedAt = new DateTime(2026, 1, 12, 21, 0, 0),
            Reference = "DEP-2026-008"
        });

        transactions.Add(new Transaction
        {
            Id = 9,
            Type = TransactionType.Depense,
            Category = "Frais de Livraison",
            Amount = 1000.00m,
            Description = "Frais Livraison Woods Farm - À REMBOURSER à Anne Holmes",
            CreatedAt = new DateTime(2026, 1, 12, 21, 0, 0),
            Reference = "DEP-2026-009"
        });

        // === RECETTES RÉELLES (du fichier recettes.txt) ===
        
        // 12/01/2026 - Ventes par Jason Parker
        transactions.Add(new Transaction
        {
            Id = 10,
            Type = TransactionType.Vente,
            Category = "Burritos - 5 unités à $695.00",
            Amount = 3475.00m,
            Description = "Servies par Jason Parker",
            CreatedAt = new DateTime(2026, 1, 12, 19, 50, 0),
            Reference = "VTE-2026-001"
        });

        transactions.Add(new Transaction
        {
            Id = 11,
            Type = TransactionType.Vente,
            Category = "Tacos - 11 unités à $746.00",
            Amount = 8206.00m,
            Description = "Servies par Jason Parker",
            CreatedAt = new DateTime(2026, 1, 12, 19, 50, 0),
            Reference = "VTE-2026-002"
        });

        // === TRANSACTIONS SUPPLÉMENTAIRES (historique simulé) ===
        
        transactions.Add(new Transaction
        {
            Id = 12,
            Type = TransactionType.Vente,
            Category = "Frites - 5 portions à $162.00",
            Amount = 810.00m,
            Description = "Servies par Jason Parker",
            CreatedAt = new DateTime(2026, 1, 10, 12, 30, 0),
            Reference = "VTE-2026-003"
        });

        transactions.Add(new Transaction
        {
            Id = 13,
            Type = TransactionType.Vente,
            Category = "Tartes aux pommes - 2 unités à $270.00",
            Amount = 540.00m,
            Description = "Servies par Anne Holmes",
            CreatedAt = new DateTime(2026, 1, 9, 20, 15, 0),
            Reference = "VTE-2026-004"
        });

        return transactions;
    }

    private void OpenAddDialog()
    {
        _isEditing = false;
        NewType = TransactionType.Vente;
        NewCategory = string.Empty;
        NewAmount = 0;
        NewDescription = string.Empty;
        NewDate = DateTime.Now;
        NewEmployeeId = null;
        IsAddEditDialogOpen = true;
    }

    private bool CanSaveTransaction()
    {
        return !string.IsNullOrWhiteSpace(NewCategory) && NewAmount > 0;
    }

    private void OpenEditDialog()
    {
        if (SelectedTransaction == null) return;

        _isEditing = true;
        _editingId = SelectedTransaction.Id;
        NewType = SelectedTransaction.Type;
        NewCategory = SelectedTransaction.Category;
        NewAmount = SelectedTransaction.Amount;
        NewDescription = SelectedTransaction.Description ?? string.Empty;
        NewDate = SelectedTransaction.CreatedAt;
        NewEmployeeId = SelectedTransaction.EmployeeId;
        IsAddEditDialogOpen = true;
    }

    private void OpenEditDialogWithParameter(object? parameter)
    {
        if (parameter is not Transaction transaction) return;

        _isEditing = true;
        _editingId = transaction.Id;
        NewType = transaction.Type;
        NewCategory = transaction.Category;
        NewAmount = transaction.Amount;
        NewDescription = transaction.Description ?? string.Empty;
        NewDate = transaction.CreatedAt;
        NewEmployeeId = transaction.EmployeeId;
        IsAddEditDialogOpen = true;
    }

    private async Task SaveTransactionAsync(object? parameter = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NewCategory) || NewAmount <= 0)
            {
                Log.Warning("Cannot save transaction: Category={Category}, Amount={Amount}", NewCategory, NewAmount);
                return;
            }

            var transaction = new Transaction
            {
                Type = NewType,
                Category = NewCategory,
                Amount = NewAmount,
                Description = NewDescription,
                UserId = _authService.CurrentUser?.Id ?? 1,
                CreatedAt = NewDate,
                EmployeeId = NewEmployeeId
            };

            Log.Information("Saving transaction: Type={Type}, Category={Category}, Amount={Amount}, UserId={UserId}, EmployeeId={EmployeeId}, Date={Date}", 
                transaction.Type, transaction.Category, transaction.Amount, transaction.UserId, transaction.EmployeeId, transaction.CreatedAt);

            if (_isEditing)
            {
                transaction.Id = _editingId;
                // Note: Pour l'instant on recrée, l'API devrait avoir un UpdateTransaction
                await _dataService.DeleteTransactionAsync(_editingId);
                var created = await _dataService.CreateTransactionAsync(transaction);
                Log.Information("Transaction updated: {Result}", created != null ? "Success" : "Failed");
                
                if (created != null)
                {
                    // Mise à jour locale
                    var existingTransaction = Transactions.FirstOrDefault(t => t.Id == _editingId);
                    if (existingTransaction != null)
                        Transactions.Remove(existingTransaction);
                    Transactions.Insert(0, created); // Ajouter en haut car c'est la plus récente
                }
            }
            else
            {
                var created = await _dataService.CreateTransactionAsync(transaction);
                Log.Information("Transaction created: {Result}", created != null ? "Success" : "Failed");
                
                if (created != null)
                {
                    // Ajouter à la collection locale
                    Transactions.Insert(0, created); // Ajouter en haut car c'est la plus récente
                }
            }

            CancelDialog();
            Log.Information(_isEditing ? "Transaction updated successfully" : "Transaction created successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving transaction");
        }
    }

    private void CancelDialog()
    {
        IsAddEditDialogOpen = false;
    }

    private async Task DeleteTransactionAsync(object? parameter)
    {
        if (parameter is Transaction transaction)
        {
            try
            {
                var success = await _dataService.DeleteTransactionAsync(transaction.Id);
                if (success)
                {
                    // Supprimer de la collection locale
                    Transactions.Remove(transaction);
                    Log.Information($"Transaction {transaction.Id} deleted");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting transaction");
            }
        }
    }
}

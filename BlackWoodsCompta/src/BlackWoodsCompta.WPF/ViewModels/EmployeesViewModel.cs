using System.Collections.ObjectModel;
using System.Windows.Input;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.Models.Enums;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels;

public class EmployeesViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IAuthService _authService;
    
    private ObservableCollection<Employee> _employees = new();
    private Employee? _selectedEmployee;
    private string _searchText = string.Empty;
    private string? _filterPosition;
    private bool _showActiveOnly = true;
    private bool _isLoading;
    private bool _isAddEditDialogOpen;
    
    // New/Edit employee properties
    private string _newName = string.Empty;
    private string _newPosition = string.Empty;
    private string _newPhone = string.Empty;
    private string _newDiscord = string.Empty;
    private string _newIdRp = string.Empty;
    private bool _newIsActive = true;
    private bool _isEditing;
    private int _editingId;

    public ObservableCollection<Employee> Employees
    {
        get => _employees;
        set => SetProperty(ref _employees, value);
    }

    public Employee? SelectedEmployee
    {
        get => _selectedEmployee;
        set => SetProperty(ref _selectedEmployee, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _ = LoadEmployeesAsync();
            }
        }
    }

    public string? FilterPosition
    {
        get => _filterPosition;
        set
        {
            if (SetProperty(ref _filterPosition, value))
            {
                _ = LoadEmployeesAsync();
            }
        }
    }

    public bool ShowActiveOnly
    {
        get => _showActiveOnly;
        set
        {
            if (SetProperty(ref _showActiveOnly, value))
            {
                _ = LoadEmployeesAsync();
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

    public string NewName
    {
        get => _newName;
        set
        {
            if (SetProperty(ref _newName, value))
            {
                (SaveEmployeeCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string NewPosition
    {
        get => _newPosition;
        set
        {
            if (SetProperty(ref _newPosition, value))
            {
                (SaveEmployeeCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string NewPhone
    {
        get => _newPhone;
        set => SetProperty(ref _newPhone, value);
    }

    public string NewDiscord
    {
        get => _newDiscord;
        set
        {
            if (SetProperty(ref _newDiscord, value))
            {
                (SaveEmployeeCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string NewIdRp
    {
        get => _newIdRp;
        set => SetProperty(ref _newIdRp, value);
    }

    public bool NewIsActive
    {
        get => _newIsActive;
        set => SetProperty(ref _newIsActive, value);
    }

    public ICommand OpenAddDialogCommand { get; }
    public ICommand OpenEditDialogCommand { get; }
    public ICommand OpenEditDialogWithParameterCommand { get; }
    public ICommand SaveEmployeeCommand { get; }
    public ICommand CancelDialogCommand { get; }
    public ICommand DeleteEmployeeCommand { get; }
    public ICommand RefreshCommand { get; }

    public List<string> Positions { get; } = new()
    {
        "PDG", "COPDG", "Manager", "Secrétaire", "Livreur", "Cuisinier"
    };

    public EmployeesViewModel(IDataService dataService, IAuthService authService)
    {
        _dataService = dataService;
        _authService = authService;

        OpenAddDialogCommand = new RelayCommand(_ => OpenAddDialog());
        OpenEditDialogCommand = new RelayCommand(_ => OpenEditDialog(), _ => SelectedEmployee != null);
        OpenEditDialogWithParameterCommand = new RelayCommand(param => OpenEditDialogWithParameter(param as Employee), param => param is Employee);
        SaveEmployeeCommand = new AsyncRelayCommand(SaveEmployeeAsync, CanSaveEmployee);
        CancelDialogCommand = new RelayCommand(_ => CancelDialog());
        DeleteEmployeeCommand = new RelayCommand(async param => await DeleteEmployeeAsync(param as Employee), param => param is Employee);
        RefreshCommand = new AsyncRelayCommand(_ => LoadEmployeesAsync());

        _ = LoadEmployeesAsync();
    }

    private void OpenAddDialog()
    {
        _isEditing = false;
        NewName = string.Empty;
        NewPosition = "PDG";
        NewPhone = string.Empty;
        NewDiscord = string.Empty;
        NewIdRp = string.Empty;
        NewIsActive = true;
        IsAddEditDialogOpen = true;
    }

    private void OpenEditDialog()
    {
        if (SelectedEmployee == null) return;

        _isEditing = true;
        _editingId = SelectedEmployee.Id;
        NewName = SelectedEmployee.Name;
        NewPosition = SelectedEmployee.Position;
        NewPhone = SelectedEmployee.Phone ?? string.Empty;
        NewDiscord = SelectedEmployee.Discord ?? string.Empty;
        NewIdRp = SelectedEmployee.IdRp ?? string.Empty;
        NewIsActive = SelectedEmployee.IsActive;
        IsAddEditDialogOpen = true;
    }

    private void OpenEditDialogWithParameter(Employee? employee)
    {
        if (employee == null) return;

        _isEditing = true;
        _editingId = employee.Id;
        NewName = employee.Name;
        NewPosition = employee.Position;
        NewPhone = employee.Phone ?? string.Empty;
        NewDiscord = employee.Discord ?? string.Empty;
        NewIdRp = employee.IdRp ?? string.Empty;
        NewIsActive = employee.IsActive;
        IsAddEditDialogOpen = true;
    }

    private bool CanSaveEmployee(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(NewName) && 
               !string.IsNullOrWhiteSpace(NewPosition) &&
               !string.IsNullOrWhiteSpace(NewDiscord);
    }

    private async Task SaveEmployeeAsync(object? parameter)
    {
        try
        {
            IsLoading = true;

            var employee = new Employee
            {
                Name = NewName,
                Position = NewPosition,
                Salary = 0, // Default value, not used anymore
                HireDate = DateTime.Now, // Set to current date
                Phone = string.IsNullOrWhiteSpace(NewPhone) ? null : NewPhone,
                Discord = string.IsNullOrWhiteSpace(NewDiscord) ? null : NewDiscord,
                IdRp = string.IsNullOrWhiteSpace(NewIdRp) ? null : NewIdRp,
                IsActive = NewIsActive
            };

            if (_isEditing)
            {
                employee.Id = _editingId;
                var success = await _dataService.UpdateEmployeeAsync(employee);
                if (success)
                {
                    Log.Information("Employee updated: {Name}", employee.Name);
                    
                    // Reload employees from database
                    await LoadEmployeesAsync();
                }
            }
            else
            {
                // Create employee
                var created = await _dataService.CreateEmployeeAsync(employee);
                if (created != null)
                {
                    Log.Information("Employee created: {Name}", created.Name);
                    
                    // Reload employees from database
                    await LoadEmployeesAsync();
                    
                    // Create user account with temporary password
                    var tempPassword = GenerateTemporaryPassword();
                    var userRole = GetUserRoleFromPosition(NewPosition);
                    
                    var user = new User
                    {
                        Username = NewDiscord,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
                        Role = userRole,
                        FullName = NewName,
                        Discord = NewDiscord,
                        IsActive = true
                    };

                    try
                    {
                        // Note: Need to add CreateUserAsync to IDataService
                        // For now we log the temp password
                        Log.Information("User account for {Name}: Discord={Discord}, TempPassword={Password}", 
                            created.Name, NewDiscord, tempPassword);
                    }
                    catch (Exception userEx)
                    {
                        Log.Error(userEx, "Error creating user account for employee");
                    }
                }
            }

            IsAddEditDialogOpen = false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error saving employee");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CancelDialog()
    {
        IsAddEditDialogOpen = false;
    }

    private async Task DeleteEmployeeAsync(Employee? employee)
    {
        if (employee == null) return;

        try
        {
            IsLoading = true;
            var success = await _dataService.DeleteEmployeeAsync(employee.Id);
            if (success)
            {
                Log.Information("Employee deleted: {Name}", employee.Name);
                await LoadEmployeesAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting employee");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    private UserRole GetUserRoleFromPosition(string position)
    {
        return position switch
        {
            "PDG" => UserRole.PDG,
            "COPDG" => UserRole.COPDG,
            "Manager" => UserRole.Manager,
            "Secrétaire" => UserRole.Secretaire,
            "Livreur" => UserRole.Livreur,
            "Cuisinier" => UserRole.Cuisinier,
            _ => UserRole.Cuisinier
        };
    }

    private async Task LoadEmployeesAsync()
    {
        try
        {
            IsLoading = true;
            var search = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText;
            var position = string.IsNullOrWhiteSpace(FilterPosition) || FilterPosition == "Tous" ? null : FilterPosition;
            var isActive = ShowActiveOnly ? true : (bool?)null;

            var employees = await _dataService.GetEmployeesAsync(search, position, isActive);
            Employees = new ObservableCollection<Employee>(employees);
            Log.Information("Loaded {Count} employees", employees.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading employees");
        }
        finally
        {
            IsLoading = false;
        }
    }
}

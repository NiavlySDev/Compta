using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;
using Serilog;

namespace BlackWoodsCompta.WPF.ViewModels
{
    public class SuppliersViewModel : INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        private ObservableCollection<Supplier> _suppliers = new();
        private ObservableCollection<Supplier> _filteredSuppliers = new();
        private string _searchText = string.Empty;
        private Supplier? _selectedSupplier;
        private bool _isAddingSupplier;
        private bool _isLoading;

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set
            {
                _suppliers = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public ObservableCollection<Supplier> FilteredSuppliers
        {
            get => _filteredSuppliers;
            set
            {
                _filteredSuppliers = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddingSupplier
        {
            get => _isAddingSupplier;
            set
            {
                _isAddingSupplier = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddSupplierCommand { get; }
        public ICommand EditSupplierCommand { get; }
        public ICommand DeleteSupplierCommand { get; }
        public ICommand SaveSupplierCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RefreshCommand { get; }

        public SuppliersViewModel(IDataService dataService)
        {
            _dataService = dataService;
            
            AddSupplierCommand = new RelayCommand(_ => AddSupplier());
            EditSupplierCommand = new RelayCommand(EditSupplier);
            DeleteSupplierCommand = new AsyncRelayCommand(DeleteSupplier);
            SaveSupplierCommand = new AsyncRelayCommand(_ => SaveSupplier());
            CancelCommand = new RelayCommand(_ => Cancel());
            RefreshCommand = new AsyncRelayCommand(_ => LoadSuppliers());

            _ = LoadSuppliers();
        }

        private async System.Threading.Tasks.Task LoadSuppliers()
        {
            try
            {
                IsLoading = true;
                var suppliers = await _dataService.GetSuppliersAsync();
                
                Suppliers.Clear();
                foreach (var supplier in suppliers)
                {
                    Suppliers.Add(supplier);
                }
                
                ApplyFilter();
                Log.Information($"Loaded {suppliers.Count} suppliers");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading suppliers");
                // Fallback to mock data if database fails
                LoadMockSuppliers();
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        private void LoadMockSuppliers()
        {
            // Charger les données réelles des fournisseurs BlackWoods
            var realSuppliers = GetRealSuppliersData();
            
            Suppliers.Clear();
            foreach (var supplier in realSuppliers)
            {
                Suppliers.Add(supplier);
            }
        }

        private ObservableCollection<Supplier> GetRealSuppliersData()
        {
            return new ObservableCollection<Supplier>
            {
                new Supplier
                {
                    Id = 1,
                    Name = "Boucherie Jackland",
                    ContactPerson = "Jack Thompson",
                    Phone = "555-1001",
                    Email = "jack@boucherie-jackland.com",
                    Address = "123 Meat Street",
                    Notes = "Fournisseur principal de viandes - Steak de bœuf $90/unité",
                    IsActive = true
                },
                new Supplier
                {
                    Id = 2,
                    Name = "Molienda Hermandad",
                    ContactPerson = "Maria Rodriguez",
                    Phone = "555-1002",
                    Email = "maria@molienda.com",
                    Address = "456 Mill Road",
                    Notes = "Farines et huiles - Farine de maïs et blé $80/kg, Huile $80/L",
                    IsActive = true
                },
                new Supplier
                {
                    Id = 3,
                    Name = "Woods Farm",
                    ContactPerson = "Robert Woods",
                    Phone = "555-1003",
                    Email = "rob@woodsfarm.com",
                    Address = "789 Farm Lane",
                    Notes = "Légumes frais - Tomates $10/kg, Laitue $15/kg, Oignons $20/kg",
                    IsActive = true
                },
                new Supplier
                {
                    Id = 4,
                    Name = "Theronis Harvest",
                    ContactPerson = "Sofia Theronis",
                    Phone = "555-1004",
                    Email = "sofia@theronis.com",
                    Address = "321 Harvest Blvd",
                    Notes = "Légumes premium - Tomates cerises $20/kg",
                    IsActive = true
                },
                new Supplier
                {
                    Id = 5,
                    Name = "Local Market Co",
                    ContactPerson = "David Brown",
                    Phone = "555-1005",
                    Email = "david@localmarket.com",
                    Address = "654 Market St",
                    Notes = "Fournisseur général - Divers produits alimentaires",
                    IsActive = true
                }
            };
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredSuppliers = new ObservableCollection<Supplier>(Suppliers);
            }
            else
            {
                var filtered = Suppliers.Where(s =>
                    s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    (s.ContactPerson?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.Phone?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                FilteredSuppliers = new ObservableCollection<Supplier>(filtered);
            }
        }

        private void AddSupplier()
        {
            SelectedSupplier = new Supplier { IsActive = true };
            IsAddingSupplier = true;
        }

        private void EditSupplier(object? parameter)
        {
            if (parameter is Supplier supplier)
            {
                SelectedSupplier = supplier;
                IsAddingSupplier = false;
            }
        }

        private async System.Threading.Tasks.Task DeleteSupplier(object? parameter)
        {
            if (parameter is Supplier supplier)
            {
                try
                {
                    var success = await _dataService.DeleteSupplierAsync(supplier.Id);
                    if (success)
                    {
                        Suppliers.Remove(supplier);
                        FilteredSuppliers.Remove(supplier);
                        Log.Information($"Supplier '{supplier.Name}' deleted successfully");
                    }
                    else
                    {
                        Log.Warning($"Failed to delete supplier '{supplier.Name}'");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error deleting supplier '{supplier.Name}'");
                }
            }
        }

        private async System.Threading.Tasks.Task SaveSupplier()
        {
            if (SelectedSupplier != null)
            {
                try
                {
                    if (IsAddingSupplier)
                    {
                        var created = await _dataService.CreateSupplierAsync(SelectedSupplier);
                        if (created != null)
                        {
                            Log.Information($"Supplier '{created.Name}' created successfully");
                            await LoadSuppliers();
                        }
                        else
                        {
                            Log.Warning("Failed to create supplier");
                        }
                    }
                    else
                    {
                        var success = await _dataService.UpdateSupplierAsync(SelectedSupplier);
                        if (success)
                        {
                            Log.Information($"Supplier '{SelectedSupplier.Name}' updated successfully");
                            await LoadSuppliers();
                        }
                        else
                        {
                            Log.Warning($"Failed to update supplier '{SelectedSupplier.Name}'");
                        }
                    }
                    
                    ApplyFilter();
                    SelectedSupplier = null;
                    IsAddingSupplier = false;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error saving supplier");
                }
            }
        }

        private void Cancel()
        {
            SelectedSupplier = null;
            IsAddingSupplier = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
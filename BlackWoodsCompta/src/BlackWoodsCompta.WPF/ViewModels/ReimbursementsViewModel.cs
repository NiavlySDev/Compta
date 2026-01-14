using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;
using Serilog;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.WPF.Helpers;
using BlackWoodsCompta.WPF.Services;

namespace BlackWoodsCompta.WPF.ViewModels
{
    public class ReimbursementsViewModel : INotifyPropertyChanged
    {
        private readonly IDataService _dataService;
        
        private ObservableCollection<EmployeeReimbursement> _reimbursements = new();
        private ObservableCollection<EmployeeReimbursement> _filteredReimbursements = new();
        private string _searchText = string.Empty;
        private EmployeeReimbursement? _selectedReimbursement;
        private bool _isAddingReimbursement;
        private bool _isLoading;
        private string? _filterStatus;
        
        // Propriétés pour le dialogue d'ajout
        private ObservableCollection<Employee> _availableEmployees = new();
        private Employee? _selectedEmployee;
        private string _newAmount = string.Empty;
        private DateTime _newRequestDate = DateTime.Now;
        private string _newDescription = string.Empty;
        private string _newNotes = string.Empty;

        // Propriétés pour les cartes de résumé
        public decimal TotalPendingAmount { get; private set; }
        public decimal TotalApprovedAmount { get; private set; }
        public decimal TotalPaidAmount { get; private set; }
        public int PendingCount { get; private set; }

        public ObservableCollection<EmployeeReimbursement> Reimbursements
        {
            get => _reimbursements;
            set
            {
                _reimbursements = value;
                OnPropertyChanged();
                ApplyFilter();
                UpdateSummaryCards();
            }
        }

        public ObservableCollection<EmployeeReimbursement> FilteredReimbursements
        {
            get => _filteredReimbursements;
            set
            {
                _filteredReimbursements = value;
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
                _ = LoadReimbursementsAsync();
            }
        }

        public EmployeeReimbursement? SelectedReimbursement
        {
            get => _selectedReimbursement;
            set
            {
                _selectedReimbursement = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddingReimbursement
        {
            get => _isAddingReimbursement;
            set
            {
                _isAddingReimbursement = value;
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

        public string? FilterStatus
        {
            get => _filterStatus;
            set
            {
                _filterStatus = value;
                OnPropertyChanged();
                _ = LoadReimbursementsAsync();
            }
        }

        // Propriétés pour le dialogue d'ajout
        public ObservableCollection<Employee> AvailableEmployees
        {
            get => _availableEmployees;
            set
            {
                _availableEmployees = value;
                OnPropertyChanged();
            }
        }

        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
            }
        }

        public string NewAmount
        {
            get => _newAmount;
            set
            {
                _newAmount = value;
                OnPropertyChanged();
            }
        }

        public DateTime NewRequestDate
        {
            get => _newRequestDate;
            set
            {
                _newRequestDate = value;
                OnPropertyChanged();
            }
        }

        public string NewDescription
        {
            get => _newDescription;
            set
            {
                _newDescription = value;
                OnPropertyChanged();
            }
        }

        public string NewNotes
        {
            get => _newNotes;
            set
            {
                _newNotes = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddReimbursementCommand { get; }
        public ICommand EditReimbursementCommand { get; }
        public ICommand DeleteReimbursementCommand { get; }
        public ICommand ApproveReimbursementCommand { get; }
        public ICommand PayReimbursementCommand { get; }
        public ICommand RejectReimbursementCommand { get; }
        public ICommand SaveReimbursementCommand { get; }
        public ICommand CancelCommand { get; }

        public ReimbursementsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            
            AddReimbursementCommand = new RelayCommand(_ => AddReimbursement());
            EditReimbursementCommand = new RelayCommand(EditReimbursement);
            DeleteReimbursementCommand = new RelayCommand(DeleteReimbursement);
            ApproveReimbursementCommand = new RelayCommand(ApproveReimbursement);
            PayReimbursementCommand = new RelayCommand(PayReimbursement);
            RejectReimbursementCommand = new RelayCommand(RejectReimbursement);
            SaveReimbursementCommand = new RelayCommand(_ => SaveReimbursement());
            CancelCommand = new RelayCommand(_ => Cancel());

            _ = LoadEmployeesAsync();
            _ = LoadReimbursementsAsync();
        }

        private async Task LoadEmployeesAsync()
        {
            try
            {
                IsLoading = true;
                var employees = await _dataService.GetEmployeesAsync();
                
                AvailableEmployees.Clear();
                foreach (var employee in employees)
                {
                    AvailableEmployees.Add(employee);
                }
                
                Log.Information("[ReimbursementsVM] Loaded {Count} employees", employees.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ReimbursementsVM] Error loading employees");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadReimbursementsAsync()
        {
            try
            {
                IsLoading = true;
                var reimbursements = await _dataService.GetEmployeeReimbursementsAsync(SearchText, FilterStatus);
                
                Reimbursements.Clear();
                foreach (var reimbursement in reimbursements)
                {
                    Reimbursements.Add(reimbursement);
                }
                
                ApplyFilter();
                UpdateSummaryCards();
                Log.Information("[ReimbursementsVM] Loaded {Count} reimbursements", reimbursements.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ReimbursementsVM] Error loading reimbursements");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateSummaryCards()
        {
            // Total à rembourser = En_Attente + Approuve
            TotalPendingAmount = Reimbursements
                .Where(r => r.Status == ReimbursementStatus.En_Attente || r.Status == ReimbursementStatus.Approuve)
                .Sum(r => r.Amount);
            
            // Total restant à rembourser = Approuve seulement
            TotalApprovedAmount = Reimbursements
                .Where(r => r.Status == ReimbursementStatus.Approuve)
                .Sum(r => r.Amount);
            
            // Total remboursé = Paye
            TotalPaidAmount = Reimbursements
                .Where(r => r.Status == ReimbursementStatus.Paye)
                .Sum(r => r.Amount);
            
            // Nombre en attente
            PendingCount = Reimbursements
                .Count(r => r.Status == ReimbursementStatus.En_Attente);

            OnPropertyChanged(nameof(TotalPendingAmount));
            OnPropertyChanged(nameof(TotalApprovedAmount));
            OnPropertyChanged(nameof(TotalPaidAmount));
            OnPropertyChanged(nameof(PendingCount));
        }

        private void ApplyFilter()
        {
            var filtered = Reimbursements.AsEnumerable();

            // Filtrage par texte
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(r =>
                    (r.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Notes?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Employee?.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            // Filtrage par statut
            if (!string.IsNullOrWhiteSpace(FilterStatus) && FilterStatus != "Tous")
            {
                if (Enum.TryParse<ReimbursementStatus>(FilterStatus, out var status))
                {
                    filtered = filtered.Where(r => r.Status == status);
                }
            }

            FilteredReimbursements = new ObservableCollection<EmployeeReimbursement>(filtered);
        }

        private void AddReimbursement()
        {
            // Réinitialiser les champs
            SelectedEmployee = null;
            NewAmount = string.Empty;
            NewRequestDate = DateTime.Now;
            NewDescription = string.Empty;
            NewNotes = string.Empty;
            
            IsAddingReimbursement = true;
        }

        private void EditReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement)
            {
                SelectedReimbursement = reimbursement;
                IsAddingReimbursement = false;
            }
        }

        private async void DeleteReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement)
            {
                try
                {
                    var success = await _dataService.DeleteEmployeeReimbursementAsync(reimbursement.Id);
                    if (success)
                    {
                        Reimbursements.Remove(reimbursement);
                        ApplyFilter();
                        UpdateSummaryCards();
                        Log.Information("[ReimbursementsVM] Deleted reimbursement ID={Id}", reimbursement.Id);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[ReimbursementsVM] Error deleting reimbursement");
                }
            }
        }

        private async void ApproveReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement && reimbursement.Status == ReimbursementStatus.En_Attente)
            {
                try
                {
                    reimbursement.Status = ReimbursementStatus.Approuve;
                    reimbursement.ApprovedDate = DateTime.Now;
                    reimbursement.UpdatedAt = DateTime.Now;
                    
                    var success = await _dataService.UpdateEmployeeReimbursementAsync(reimbursement);
                    if (success)
                    {
                        ApplyFilter();
                        UpdateSummaryCards();
                        Log.Information("[ReimbursementsVM] Approved reimbursement ID={Id}", reimbursement.Id);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[ReimbursementsVM] Error approving reimbursement");
                }
            }
        }

        private async void PayReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement && reimbursement.Status == ReimbursementStatus.Approuve)
            {
                try
                {
                    reimbursement.Status = ReimbursementStatus.Paye;
                    reimbursement.PaidDate = DateTime.Now;
                    reimbursement.UpdatedAt = DateTime.Now;
                    
                    var success = await _dataService.UpdateEmployeeReimbursementAsync(reimbursement);
                    if (success)
                    {
                        ApplyFilter();
                        UpdateSummaryCards();
                        Log.Information("[ReimbursementsVM] Paid reimbursement ID={Id}", reimbursement.Id);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[ReimbursementsVM] Error paying reimbursement");
                }
            }
        }

        private async void RejectReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement && reimbursement.Status == ReimbursementStatus.En_Attente)
            {
                try
                {
                    var success = await _dataService.DeleteEmployeeReimbursementAsync(reimbursement.Id);
                    if (success)
                    {
                        Reimbursements.Remove(reimbursement);
                        ApplyFilter();
                        UpdateSummaryCards();
                        Log.Information("[ReimbursementsVM] Rejected (deleted) reimbursement ID={Id}", reimbursement.Id);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[ReimbursementsVM] Error rejecting reimbursement");
                }
            }
        }

        private async void SaveReimbursement()
        {
            if (SelectedEmployee != null && !string.IsNullOrWhiteSpace(NewAmount) && !string.IsNullOrWhiteSpace(NewDescription))
            {
                if (decimal.TryParse(NewAmount, out var amount))
                {
                    try
                    {
                        var newReimbursement = new EmployeeReimbursement
                        {
                            EmployeeId = SelectedEmployee.Id,
                            Employee = SelectedEmployee,
                            Amount = amount,
                            Description = NewDescription,
                            RequestDate = NewRequestDate,
                            Status = ReimbursementStatus.En_Attente,
                            Notes = NewNotes,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        var created = await _dataService.CreateEmployeeReimbursementAsync(newReimbursement);
                        if (created != null)
                        {
                            Reimbursements.Add(created);
                            ApplyFilter();
                            UpdateSummaryCards();
                            Cancel();
                            Log.Information("[ReimbursementsVM] Created reimbursement ID={Id}", created.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "[ReimbursementsVM] Error creating reimbursement");
                    }
                }
            }
        }

        private void Cancel()
        {
            // Réinitialiser les champs du dialogue
            SelectedEmployee = null;
            NewAmount = string.Empty;
            NewRequestDate = DateTime.Now;
            NewDescription = string.Empty;
            NewNotes = string.Empty;
            
            SelectedReimbursement = null;
            IsAddingReimbursement = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
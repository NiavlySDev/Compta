using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.WPF.Helpers;

namespace BlackWoodsCompta.WPF.ViewModels
{
    public class ReimbursementsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<EmployeeReimbursement> _reimbursements = new();
        private ObservableCollection<EmployeeReimbursement> _filteredReimbursements = new();
        private string _searchText = string.Empty;
        private EmployeeReimbursement? _selectedReimbursement;
        private bool _isAddingReimbursement;

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
                ApplyFilter();
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

        public ICommand AddReimbursementCommand { get; }
        public ICommand EditReimbursementCommand { get; }
        public ICommand DeleteReimbursementCommand { get; }
        public ICommand ApproveReimbursementCommand { get; }
        public ICommand PayReimbursementCommand { get; }
        public ICommand RejectReimbursementCommand { get; }
        public ICommand SaveReimbursementCommand { get; }
        public ICommand CancelCommand { get; }

        public ReimbursementsViewModel()
        {
            AddReimbursementCommand = new RelayCommand(_ => AddReimbursement());
            EditReimbursementCommand = new RelayCommand(EditReimbursement);
            DeleteReimbursementCommand = new RelayCommand(DeleteReimbursement);
            ApproveReimbursementCommand = new RelayCommand(ApproveReimbursement);
            PayReimbursementCommand = new RelayCommand(PayReimbursement);
            RejectReimbursementCommand = new RelayCommand(RejectReimbursement);
            SaveReimbursementCommand = new RelayCommand(_ => SaveReimbursement());
            CancelCommand = new RelayCommand(_ => Cancel());

            LoadReimbursements();
        }

        private void LoadReimbursements()
        {
            // Charger les données réelles des remboursements BlackWoods
            var realReimbursements = GetRealReimbursementsData();
            
            Reimbursements.Clear();
            foreach (var reimbursement in realReimbursements)
            {
                Reimbursements.Add(reimbursement);
            }
        }

        private ObservableCollection<EmployeeReimbursement> GetRealReimbursementsData()
        {
            return new ObservableCollection<EmployeeReimbursement>
            {
                // Anne Holmes - Remboursement principal de $17,000 pour achats Woods Farm
                new EmployeeReimbursement
                {
                    Id = 1,
                    EmployeeId = 1,
                    Description = "Achats Woods Farm - Légumes pour production (Anne Holmes)",
                    Amount = 17000.00m,
                    RequestDate = new DateTime(2026, 1, 15),
                    Status = ReimbursementStatus.En_Attente,
                    Notes = "Remboursement urgent - Achats fournisseur Woods Farm"
                },
                new EmployeeReimbursement
                {
                    Id = 2,
                    EmployeeId = 2,
                    Description = "Remboursement frais de transport - Livraisons (John Smith)",
                    Amount = 250.00m,
                    RequestDate = new DateTime(2026, 1, 10),
                    Status = ReimbursementStatus.Approuve,
                    ApprovedDate = new DateTime(2026, 1, 15),
                    Notes = "Transport approuvé"
                },
                new EmployeeReimbursement
                {
                    Id = 3,
                    EmployeeId = 3,
                    Description = "Équipement de cuisine - Ustensiles (Sarah Johnson)",
                    Amount = 180.00m,
                    RequestDate = new DateTime(2026, 1, 8),
                    ApprovedDate = new DateTime(2026, 1, 12),
                    PaidDate = new DateTime(2026, 1, 20),
                    Status = ReimbursementStatus.Paye,
                    Notes = "Équipement payé"
                },
                new EmployeeReimbursement
                {
                    Id = 4,
                    EmployeeId = 4,
                    Description = "Formation hygiène alimentaire (Mike Wilson)",
                    Amount = 150.00m,
                    RequestDate = new DateTime(2026, 1, 5),
                    Status = ReimbursementStatus.Rejete,
                    Notes = "Budget formation dépassé pour ce trimestre"
                },
                new EmployeeReimbursement
                {
                    Id = 5,
                    EmployeeId = 5,
                    Description = "Réparation équipement - Friteuse (Lisa Brown)",
                    Amount = 320.00m,
                    RequestDate = new DateTime(2026, 1, 12),
                    Status = ReimbursementStatus.En_Attente,
                    Notes = "Réparation urgente nécessaire"
                }
            };
        }

        private void UpdateSummaryCards()
        {
            TotalPendingAmount = Reimbursements.Where(r => r.Status == ReimbursementStatus.En_Attente).Sum(r => r.Amount);
            TotalApprovedAmount = Reimbursements.Where(r => r.Status == ReimbursementStatus.Approuve).Sum(r => r.Amount);
            TotalPaidAmount = Reimbursements.Where(r => r.Status == ReimbursementStatus.Paye).Sum(r => r.Amount);
            PendingCount = Reimbursements.Count(r => r.Status == ReimbursementStatus.En_Attente);

            OnPropertyChanged(nameof(TotalPendingAmount));
            OnPropertyChanged(nameof(TotalApprovedAmount));
            OnPropertyChanged(nameof(TotalPaidAmount));
            OnPropertyChanged(nameof(PendingCount));
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredReimbursements = new ObservableCollection<EmployeeReimbursement>(Reimbursements);
            }
            else
            {
                var filtered = Reimbursements.Where(r =>
                    (r.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.Notes?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                FilteredReimbursements = new ObservableCollection<EmployeeReimbursement>(filtered);
            }
        }

        private void AddReimbursement()
        {
            SelectedReimbursement = new EmployeeReimbursement 
            { 
                RequestDate = DateTime.Now,
                Status = ReimbursementStatus.En_Attente
            };
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

        private void DeleteReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement)
            {
                Reimbursements.Remove(reimbursement);
                ApplyFilter();
                UpdateSummaryCards();
            }
        }

        private void ApproveReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement && reimbursement.Status == ReimbursementStatus.En_Attente)
            {
                reimbursement.Status = ReimbursementStatus.Approuve;
                reimbursement.ApprovedDate = DateTime.Now;
                reimbursement.UpdatedAt = DateTime.Now;
                UpdateSummaryCards();
                OnPropertyChanged(nameof(FilteredReimbursements));
            }
        }

        private void PayReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement && reimbursement.Status == ReimbursementStatus.Approuve)
            {
                reimbursement.Status = ReimbursementStatus.Paye;
                reimbursement.PaidDate = DateTime.Now;
                reimbursement.UpdatedAt = DateTime.Now;
                UpdateSummaryCards();
                OnPropertyChanged(nameof(FilteredReimbursements));
            }
        }

        private void RejectReimbursement(object? parameter)
        {
            if (parameter is EmployeeReimbursement reimbursement && reimbursement.Status == ReimbursementStatus.En_Attente)
            {
                reimbursement.Status = ReimbursementStatus.Rejete;
                reimbursement.UpdatedAt = DateTime.Now;
                // Dans une vraie application, on demanderait la raison du rejet
                reimbursement.Notes = "Rejeté par l'administrateur";
                UpdateSummaryCards();
                OnPropertyChanged(nameof(FilteredReimbursements));
            }
        }

        private void SaveReimbursement()
        {
            if (SelectedReimbursement != null)
            {
                if (IsAddingReimbursement)
                {
                    SelectedReimbursement.Id = Reimbursements.Count > 0 ? Reimbursements.Max(r => r.Id) + 1 : 1;
                    Reimbursements.Add(SelectedReimbursement);
                }
                
                ApplyFilter();
                UpdateSummaryCards();
                SelectedReimbursement = null;
                IsAddingReimbursement = false;
            }
        }

        private void Cancel()
        {
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
namespace BlackWoodsCompta.Models.Entities;

public class EmployeeReimbursement
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.Now;
    public DateTime? ApprovedDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public ReimbursementStatus Status { get; set; } = ReimbursementStatus.En_Attente;
    public string? TransactionIds { get; set; } // IDs des transactions séparés par virgules
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation property
    public Employee? Employee { get; set; }
}

public enum ReimbursementStatus
{
    En_Attente,
    Approuve,
    Paye,
    Rejete
}
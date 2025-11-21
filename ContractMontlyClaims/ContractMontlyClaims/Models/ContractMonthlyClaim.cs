using System.ComponentModel.DataAnnotations;

namespace ContractMontlyClaims.Models
{
    public enum ClaimStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Settled = 3
    }

    public class ContractMonthlyClaim
    {
        public int Id { get; set; }

        public string? LecturerId { get; set; }

        public string? LecturerName { get; set; }

        [Required]
        [Range(0.25, 300)]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal HourlyRate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string? AttachmentFileName { get; set; }

        public string? AttachmentOriginalFileName { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public decimal TotalAmount => HoursWorked * HourlyRate;
    }
}

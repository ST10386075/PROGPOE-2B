using System.Collections.Generic;

namespace ContractMontlyClaims.Models
{
    public class CoordinatorDashboardViewModel
    {
        public int PendingCount { get; set; }

        public int ClaimsThisMonth { get; set; }

        public decimal AmountPending { get; set; }

        public int ApprovedThisMonth { get; set; }

        public IReadOnlyList<ContractMonthlyClaim> PendingClaims { get; set; } = new List<ContractMonthlyClaim>();
    }
}

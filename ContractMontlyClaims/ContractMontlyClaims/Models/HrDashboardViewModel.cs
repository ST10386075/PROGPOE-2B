using System.Collections.Generic;

namespace ContractMontlyClaims.Models
{
    public class HrDashboardViewModel
    {
        public int ApprovedCount { get; set; }

        public decimal ApprovedTotal { get; set; }

        public int SettledCount { get; set; }

        public decimal SettledTotal { get; set; }

        public IReadOnlyList<ContractMonthlyClaim> RecentApproved { get; set; } = new List<ContractMonthlyClaim>();

        public IReadOnlyList<ContractMonthlyClaim> RecentSettled { get; set; } = new List<ContractMonthlyClaim>();
    }
}

using System.Collections.Generic;

namespace ContractMontlyClaims.Models
{
    public class LecturerDashboardViewModel
    {
        public ApplicationUser Lecturer { get; set; } = new ApplicationUser();

        public IReadOnlyList<ContractMonthlyClaim> RecentClaims { get; set; } = new List<ContractMonthlyClaim>();
    }
}

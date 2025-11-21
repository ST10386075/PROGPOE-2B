using ContractMontlyClaims.Models;
using ContractMontlyClaims.Services;
using Xunit;

namespace ContractMontlyClaims.Tests
{
    public class ContractMonthlyClaimServiceTests
    {
        [Fact]
        public void Create_AssignsIdAndInitialStatus()
        {
            var service = new InMemoryContractMonthlyClaimService();
            var claim = new ContractMonthlyClaim
            {
                HoursWorked = 10,
                HourlyRate = 100
            };

            var created = service.Create(claim);

            Assert.Equal(1, created.Id);
            Assert.Equal(ClaimStatus.Pending, created.Status);
            Assert.Equal(1000, created.TotalAmount);
        }

        [Fact]
        public void UpdateStatus_ChangesStatusAndUpdatedAt()
        {
            var service = new InMemoryContractMonthlyClaimService();
            var claim = new ContractMonthlyClaim
            {
                HoursWorked = 5,
                HourlyRate = 200
            };

            var created = service.Create(claim);
            var result = service.UpdateStatus(created.Id, ClaimStatus.Approved);
            var updated = service.GetById(created.Id);

            Assert.True(result);
            Assert.NotNull(updated);
            Assert.Equal(ClaimStatus.Approved, updated!.Status);
            Assert.NotNull(updated.UpdatedAt);
        }

        [Fact]
        public void GetPending_ReturnsOnlyPendingClaims()
        {
            var service = new InMemoryContractMonthlyClaimService();

            var first = service.Create(new ContractMonthlyClaim
            {
                HoursWorked = 3,
                HourlyRate = 150
            });

            var second = service.Create(new ContractMonthlyClaim
            {
                HoursWorked = 4,
                HourlyRate = 200
            });

            service.UpdateStatus(second.Id, ClaimStatus.Approved);

            var pending = service.GetPending();

            Assert.Single(pending);
            Assert.Contains(pending, c => c.Id == first.Id);
        }

        [Fact]
        public void UpdateStatus_ReturnsFalseWhenClaimNotFound()
        {
            var service = new InMemoryContractMonthlyClaimService();

            var result = service.UpdateStatus(999, ClaimStatus.Approved);

            Assert.False(result);
        }
    }
}

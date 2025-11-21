using System.Linq;
using ContractMontlyClaims.Models;

namespace ContractMontlyClaims.Services
{
    public interface IContractMonthlyClaimService
    {
        IReadOnlyList<ContractMonthlyClaim> GetAll();
        IReadOnlyList<ContractMonthlyClaim> GetPending();
        ContractMonthlyClaim? GetById(int id);
        ContractMonthlyClaim Create(ContractMonthlyClaim claim);
        bool UpdateStatus(int id, ClaimStatus status);
    }

    public class InMemoryContractMonthlyClaimService : IContractMonthlyClaimService
    {
        private readonly List<ContractMonthlyClaim> _claims = new();
        private int _nextId = 1;
        private readonly object _lock = new();

        public ContractMonthlyClaim Create(ContractMonthlyClaim claim)
        {
            lock (_lock)
            {
                claim.Id = _nextId++;
                claim.Status = ClaimStatus.Pending;
                claim.CreatedAt = DateTime.UtcNow;
                _claims.Add(claim);
                return claim;
            }
        }

        public IReadOnlyList<ContractMonthlyClaim> GetAll()
        {
            lock (_lock)
            {
                return _claims
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
            }
        }

        public IReadOnlyList<ContractMonthlyClaim> GetPending()
        {
            lock (_lock)
            {
                return _claims
                    .Where(c => c.Status == ClaimStatus.Pending)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();
            }
        }

        public ContractMonthlyClaim? GetById(int id)
        {
            lock (_lock)
            {
                return _claims.FirstOrDefault(c => c.Id == id);
            }
        }

        public bool UpdateStatus(int id, ClaimStatus status)
        {
            lock (_lock)
            {
                var claim = _claims.FirstOrDefault(c => c.Id == id);
                if (claim == null)
                {
                    return false;
                }

                claim.Status = status;
                claim.UpdatedAt = DateTime.UtcNow;
                return true;
            }
        }
    }
}

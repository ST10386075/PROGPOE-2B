using System;
using System.Collections.Generic;
using System.Linq;
using ContractMontlyClaims.Data;
using ContractMontlyClaims.Models;

namespace ContractMontlyClaims.Services
{
    public class EfContractMonthlyClaimService : IContractMonthlyClaimService
    {
        private readonly ApplicationDbContext _db;

        public EfContractMonthlyClaimService(ApplicationDbContext db)
        {
            _db = db;
        }

        public ContractMonthlyClaim Create(ContractMonthlyClaim claim)
        {
            claim.Status = ClaimStatus.Pending;
            claim.CreatedAt = DateTime.UtcNow;

            _db.ContractMonthlyClaims.Add(claim);
            _db.SaveChanges();
            return claim;
        }

        public IReadOnlyList<ContractMonthlyClaim> GetAll()
        {
            return _db.ContractMonthlyClaims
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public IReadOnlyList<ContractMonthlyClaim> GetPending()
        {
            return _db.ContractMonthlyClaims
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public ContractMonthlyClaim? GetById(int id)
        {
            return _db.ContractMonthlyClaims
                .FirstOrDefault(c => c.Id == id);
        }

        public bool UpdateStatus(int id, ClaimStatus status)
        {
            var claim = _db.ContractMonthlyClaims
                .FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return false;
            }

            claim.Status = status;
            claim.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();
            return true;
        }
    }
}

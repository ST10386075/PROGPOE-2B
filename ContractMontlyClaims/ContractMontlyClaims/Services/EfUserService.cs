using System;
using System.Linq;
using ContractMontlyClaims.Data;
using ContractMontlyClaims.Models;

namespace ContractMontlyClaims.Services
{
    public class EfUserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public EfUserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public ApplicationUser? ValidateUser(string userId, string password, UserRole role)
        {
            return _db.Users
                .FirstOrDefault(u => u.Id.Equals(userId, StringComparison.OrdinalIgnoreCase)
                                     && u.Password == password
                                     && u.Role == role);
        }

        public ApplicationUser? GetById(string id)
        {
            return _db.Users
                .FirstOrDefault(u => u.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public ApplicationUser Create(ApplicationUser user)
        {
            if (_db.Users.Any(u => u.Id.Equals(user.Id, StringComparison.OrdinalIgnoreCase)))
            {
                return user;
            }

            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }

        public void Update(ApplicationUser user)
        {
            var existing = _db.Users
                .FirstOrDefault(u => u.Id.Equals(user.Id, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                return;
            }

            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.Phone = user.Phone;
            existing.Department = user.Department;
            existing.Role = user.Role;
            existing.Password = user.Password;

            _db.SaveChanges();
        }
    }
}

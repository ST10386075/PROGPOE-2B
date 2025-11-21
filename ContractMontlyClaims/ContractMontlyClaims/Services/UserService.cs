using System;
using System.Collections.Generic;
using System.Linq;
using ContractMontlyClaims.Models;

namespace ContractMontlyClaims.Services
{
    public interface IUserService
    {
        ApplicationUser? ValidateUser(string userId, string password, UserRole role);

        ApplicationUser? GetById(string id);

        ApplicationUser Create(ApplicationUser user);

        void Update(ApplicationUser user);
    }

    public class InMemoryUserService : IUserService
    {
        private readonly List<ApplicationUser> _users = new()
        {
            new ApplicationUser
            {
                Id = "LEC001",
                FullName = "Dr. John Smith",
                Email = "john.smith@university.edu",
                Phone = "+27 11 000 0001",
                Department = "Computer Science",
                Role = UserRole.Lecturer,
                Password = "Password123!"
            },
            new ApplicationUser
            {
                Id = "PC001",
                FullName = "Ms. Jane Doe",
                Email = "jane.doe@university.edu",
                Phone = "+27 11 000 0002",
                Department = "Information Technology",
                Role = UserRole.ProgrammeCoordinator,
                Password = "Password123!"
            }
        };

        private readonly object _lock = new();

        public ApplicationUser? ValidateUser(string userId, string password, UserRole role)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u =>
                    u.Id.Equals(userId, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password &&
                    u.Role == role);
            }
        }

        public ApplicationUser? GetById(string id)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            }
        }

        public ApplicationUser Create(ApplicationUser user)
        {
            lock (_lock)
            {
                if (_users.Any(u => u.Id.Equals(user.Id, StringComparison.OrdinalIgnoreCase)))
                {
                    return user;
                }

                _users.Add(user);
                return user;
            }
        }

        public void Update(ApplicationUser user)
        {
            lock (_lock)
            {
                var existing = _users.FirstOrDefault(u => u.Id.Equals(user.Id, StringComparison.OrdinalIgnoreCase));
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
            }
        }
    }
}

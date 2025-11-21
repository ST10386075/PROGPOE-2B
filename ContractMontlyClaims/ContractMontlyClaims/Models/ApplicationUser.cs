using System.ComponentModel.DataAnnotations;

namespace ContractMontlyClaims.Models
{
    public enum UserRole
    {
        Lecturer,
        ProgrammeCoordinator
    }

    public class ApplicationUser
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        // Demo-only password storage. For production use ASP.NET Core Identity with hashed passwords.
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ContractMontlyClaims.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Staff ID")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone number")]
        public string Phone { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

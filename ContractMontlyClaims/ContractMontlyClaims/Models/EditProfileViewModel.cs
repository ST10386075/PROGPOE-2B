using System.ComponentModel.DataAnnotations;

namespace ContractMontlyClaims.Models
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "Staff ID")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone number")]
        public string Phone { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }
    }
}

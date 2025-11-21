using System.ComponentModel.DataAnnotations;

namespace ContractMontlyClaims.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Staff ID")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        public string? ReturnUrl { get; set; }
    }
}

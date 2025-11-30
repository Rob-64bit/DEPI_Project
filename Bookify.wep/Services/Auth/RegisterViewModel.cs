using System.ComponentModel.DataAnnotations;

namespace Bookify.wep.Models.Auth
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "User name")]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 chars.")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}

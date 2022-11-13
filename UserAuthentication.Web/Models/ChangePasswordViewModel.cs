using System.ComponentModel.DataAnnotations;

namespace UserAuthentication.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", 
            ErrorMessage = "Should have 8 characters and at least one upper case, one lower case, one number, one special character.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Retype Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$",
            ErrorMessage = "Should have 8 characters and at least one upper case, one lower case, one number, one special character.")]
        [DataType(DataType.Password)]
        public string NewPasswordRetype { get; set; }
    }
}

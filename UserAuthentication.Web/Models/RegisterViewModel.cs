using System.ComponentModel.DataAnnotations;

namespace UserAuthentication.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email Address")]        
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

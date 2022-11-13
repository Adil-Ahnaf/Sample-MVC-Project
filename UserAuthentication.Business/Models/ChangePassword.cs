using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAuthentication.Business.Models
{
    public class ChangePassword
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordRetype { get; set; }
        public string Email { get; set; }
        public bool IsSuccess { get; set; }
    }
}

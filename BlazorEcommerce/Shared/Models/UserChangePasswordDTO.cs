using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared.Models
{
    public class UserChangePasswordDTO
    {
        [Required(ErrorMessage = "Pasword is required"), StringLength(100, MinimumLength = 6, ErrorMessage = "Password have to shorter than 100 characters & minimum characters is 6")] 
        public string NewPassword { get; set; } = string.Empty;
        [Compare("NewPassword", ErrorMessage = "The passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

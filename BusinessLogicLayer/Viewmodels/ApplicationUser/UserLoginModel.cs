using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Tài khoản là bắt buộc.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        public string? PassWord { get; set; }
    }
}

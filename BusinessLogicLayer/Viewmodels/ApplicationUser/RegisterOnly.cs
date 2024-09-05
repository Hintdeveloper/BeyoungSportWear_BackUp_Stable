
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class RegisterOnly
    {
        [RegularExpression(@"^(\p{L}+\s){1,3}\p{L}+$", ErrorMessage = "Yêu cầu đầy đủ họ tên và nhập đúng định dạng, ví dụ: Nguyễn Văn A")]
        [Required(ErrorMessage = "Họ và tên không được bỏ trống")]

        public string FirstAndLastName { get; set; } = null!;
        [Required(ErrorMessage = "Tên người dùng không được bỏ trống")]
        public string Username { get; set; } = null!;
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z.-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$", ErrorMessage = "Yêu cầu nhập email đúng định dạng")]
        [Required(ErrorMessage = "Email không được để trống")]
        public string Email { get; set; } = null!;
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và gồm 10 chữ số.")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string PhoneNumber { get; set; }
        public int Gender { get; set; }
    }
}

using BusinessLogicLayer.Viewmodels.Address;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class RegisterUser
    {
        public DateTime JoinDate { get; set; }

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
        public IFormFile? Images { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Mật khẩu nhập lại không được để trống")]
        public string ConfirmPassword { get; set; } = null!;
        [Required(ErrorMessage = "Thành phố không được để trống")]
        public string City { get; set; } = null!;//Thành phố
        [Required(ErrorMessage = "Quận huyện không được để trống")]
        public string DistrictCounty { get; set; } = null!;//Quận
        [Required(ErrorMessage = "Xã phường không được để trống")]
        public string Commune { get; set; } = null!;//Xã
        [Required(ErrorMessage = "Địa chỉ cụ thể không được để trống")]
        public string SpecificAddress { get; set; } = null!;//Cụ thể
        public AddressCreateVM? AddressCreateVM { get; set; }
    }
}

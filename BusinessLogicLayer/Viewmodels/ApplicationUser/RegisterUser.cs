using BusinessLogicLayer.Viewmodels.Address;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class RegisterUser
    {
        [RegularExpression(@"^([a-zA-ZÀ-ỹ]+\s){2,3}[a-zA-ZÀ-ỹ]+$", ErrorMessage = "Yêu cầu đầy đủ họ tên và nhập đúng định dạng, ví dụ: Nguyễn Văn A")]
        public string FirstAndLastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$", ErrorMessage = "Yêu cầu nhập email đúng định dạng")]
        public string Email { get; set; } = null!;
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và gồm 10 chữ số.")]
        public string PhoneNumber { get; set; }
        public int Gender { get; set; }
        public IFormFile? Images { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string City { get; set; } = null!;//Thành phố
        public string DistrictCounty { get; set; } = null!;//Quận
        public string Commune { get; set; } = null!;//Xã
        public string SpecificAddress { get; set; } = null!;//Cụ thể
        public AddressCreateVM? AddressCreateVM { get; set; }
    }
}

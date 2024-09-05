using BusinessLogicLayer.Viewmodels.Address;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class UserUpdateVM
    {
       
        public string? ModifiedBy { get; set; }
        [RegularExpression(@"^([a-zA-ZÀ-ỹ]+\s){2,3}[a-zA-ZÀ-ỹ]+$", ErrorMessage = "Yêu cầu đầy đủ họ tên và nhập đúng định dạng, ví dụ: Nguyễn Văn A")]
        [Required(ErrorMessage = "Họ và tên không được bỏ trống")]
        public string? FirstAndLastName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$", ErrorMessage = "Yêu cầu nhập email đúng định dạng")]
        [Required(ErrorMessage = "Email không được để trống")]
        public string? Email { get; set; }
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và gồm 10 chữ số.")]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string? PhoneNumber { get; set; }
        public int? Gender { get; set; }
        public IFormFile? Images { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public AddressUpdateVM? AddressUpdateVM { get; set; }
    }
}

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
        public string? FirstAndLastName { get; set; } 
        public string? Email { get; set; } = null!;
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và gồm 10 chữ số.")]
        public string? PhoneNumber { get; set; }
        public int? Gender { get; set; }
        public IFormFile? Images { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public AddressUpdateVM? AddressUpdateVM { get; set; }
    }
}

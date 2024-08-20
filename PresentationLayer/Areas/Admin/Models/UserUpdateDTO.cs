using BusinessLogicLayer.Viewmodels.Address;

namespace PresentationLayer.Areas.Admin.Models
{
    public class UserUpdateDTO
    {
        public string? ModifiedBy { get; set; }
        public string? FirstAndLastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Gender { get; set; }
        public string? Images { get; set; }  // Đây sẽ là URL hình ảnh từ API
        public DateTime? DateOfBirth { get; set; }
        public AddressUpdateVM? AddressUpdateVM { get; set; }
    }
}

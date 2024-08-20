using BusinessLogicLayer.Viewmodels.Address;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class RegisterUser
    {
        public string FirstAndLastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
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

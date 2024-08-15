using BusinessLogicLayer.Viewmodels.Address;

namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class UserDataVM
    {
        public string ID { get; set; } = null!;
        public string FirstAndLastName { get; set; } = null!;
        public int Gender { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Images { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }
        public string? RoleName { get; set; } = null!;
        public int Status { get; set; } = 1;
        public List<AddressVM>? AddressVMs { get; set; }
    }
}

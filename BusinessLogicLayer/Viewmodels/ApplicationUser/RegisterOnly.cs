
namespace BusinessLogicLayer.Viewmodels.ApplicationUser
{
    public class RegisterOnly
    {
        public string FirstAndLastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public int Gender { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;


namespace BusinessLogicLayer.Viewmodels.Address
{
    public class AddressCreateVM
    {
        public string? FirstAndLastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gmail { get; set; }
        public bool IsDefault { get; set; }
        public string CreateBy { get; set; } = null!;
        public string IDUser { get; set; }
        public string City { get; set; } = null!;//Thành phố
        public string DistrictCounty { get; set; } = null!;//Quận
        public string Commune { get; set; } = null!;//Xã
        public string SpecificAddress { get; set; } = null!;//Cụ thể
    }
}

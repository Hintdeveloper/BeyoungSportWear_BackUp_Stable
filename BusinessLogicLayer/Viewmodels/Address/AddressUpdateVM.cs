namespace BusinessLogicLayer.Viewmodels.Address
{
    public class AddressUpdateVM
    {
        public string? ModifiedBy { get; set; }
        public string? FirstAndLastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gmail { get; set; }
        public bool IsDefault { get; set; }
        public string? IDUser { get; set; }
        public string? City { get; set; }
        public string? DistrictCounty { get; set; }
        public string? Commune { get; set; }
        public string? SpecificAddress { get; set; }
        public int Status { get; set; }
    }
}

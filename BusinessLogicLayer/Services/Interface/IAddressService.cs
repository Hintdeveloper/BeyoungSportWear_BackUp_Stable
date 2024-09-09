
using BusinessLogicLayer.Viewmodels.Address;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IAddressService
    {
        public Task<List<AddressVM>> GetAllAsync();
        public Task<List<AddressVM>> GetAllActiveAsync();
        public Task<AddressVM> GetByIDAsync(Guid ID);
        public Task<List<AddressVM>> GetAddressByIDUserAsync(string IDUser);
        public Task<bool> CreateAsync(AddressCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserDelete);
        public Task<bool> UpdateAsync(Guid ID, AddressUpdateVM request);
        public Task<bool> SetDefaultAddressAsync(Guid ID, string IDUser);
    }
}

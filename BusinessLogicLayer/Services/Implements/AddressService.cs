using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Address;
using CloudinaryDotNet.Core;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services.Implements
{
    public partial class AddressService : IAddressService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public AddressService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(AddressCreateVM request)
        {
            if (request != null)
            {
                //bool isDuplicate = await _dbcontext.Address.AnyAsync(a => a.PhoneNumber == request.PhoneNumber && a.Gmail == request.Gmail);
                //if (isDuplicate)
                //{
                //    throw new Exception("Địa chỉ đã tồn tại với cùng số điện thoại và Gmail.");
                //}
                if (request.IsDefault)
                {
                    var existingAddresses = await _dbcontext.Address
                        .Where(a => a.IDUser == request.IDUser && a.IsDefault)
                        .ToListAsync();

                    foreach (var address in existingAddresses)
                    {
                        address.IsDefault = false;
                    }

                    _dbcontext.Address.UpdateRange(existingAddresses);
                }
                var Obj = new Address()
                {
                    ID = Guid.NewGuid(),
                    FirstAndLastName = request.FirstAndLastName,
                    PhoneNumber = request.PhoneNumber,
                    Gmail = request.Gmail,
                    City = request.City,
                    IDUser = request.IDUser,
                    DistrictCounty = request.DistrictCounty,
                    Commune = request.Commune,
                    SpecificAddress = request.SpecificAddress,
                    IsDefault = request.IsDefault,
                    Status = 1,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,

                };
                await _dbcontext.Address.AddRangeAsync(Obj);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<AddressVM>> GetAddressByIDUserAsync(string IDUser)
        {
            var addresses = await _dbcontext.Address
                .Where(a => a.IDUser == IDUser && a.Status == 1)
                .OrderByDescending(a => a.IsDefault == true)
                .ToListAsync();
            return _mapper.Map<List<AddressVM>>(addresses);
        }

        public async Task<List<AddressVM>> GetAllActiveAsync()
        {
            var addresses = await _dbcontext.Address
                .Where(a => a.Status == 1)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
            return _mapper.Map<List<AddressVM>>(addresses);
        }

        public async Task<List<AddressVM>> GetAllAsync()
        {
            var addresses = await _dbcontext.Address.ToListAsync();
            return _mapper.Map<List<AddressVM>>(addresses);
        }

        public async Task<AddressVM> GetByIDAsync(Guid ID)
        {
            var address = await _dbcontext.Address.FindAsync(ID);
            return _mapper.Map<AddressVM>(address);
        }

        public async Task<bool> RemoveAsync(Guid ID, string IDUserDelete)
        {
            var address = await _dbcontext.Address.FindAsync(ID);
            if (address != null)
            {

                bool wasDefault = address.IsDefault;
                _dbcontext.Address.Remove(address);
                await _dbcontext.SaveChangesAsync();

                if (wasDefault)
                {
                    var nextDefaultAddress = await _dbcontext.Address
                        .Where(a => a.IDUser == address.IDUser)
                        .OrderBy(a => a.CreateDate)
                        .FirstOrDefaultAsync();

                    if (nextDefaultAddress != null)
                    {
                        nextDefaultAddress.IsDefault = true;
                        _dbcontext.Address.Update(nextDefaultAddress);
                        await _dbcontext.SaveChangesAsync();
                    }
                }

                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(Guid ID, AddressUpdateVM request)
        {
            var address = await _dbcontext.Address.FindAsync(ID);
            if (address != null)
            {
                address.IDUser = request.IDUser;
                address.FirstAndLastName = request.FirstAndLastName;
                address.Gmail = request.Gmail;
                address.PhoneNumber = request.PhoneNumber;
                address.City = request.City;
                address.Commune = request.Commune;
                address.DistrictCounty = request.DistrictCounty;
                address.SpecificAddress = request.SpecificAddress;
                address.IsDefault = request.IsDefault;
                address.ModifiedDate = DateTime.Now;
                address.ModifiedBy = request.ModifiedBy;

                _dbcontext.Address.Update(address);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}

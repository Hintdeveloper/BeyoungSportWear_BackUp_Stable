using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IVoucherMServiece
    {
        public Task<bool> Create(CreateVoucherVM request);
        public Task<List<GetAllVoucherVM>> GetAllAsync();

        public Task<List<GetAllVoucherVM>> GetAll();
        public Task<GetAllVoucherVM> GetByIDAsync(Guid ID);
        public Task<bool> UpdateAsync(Guid ID, UpdateVoucherVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserdelete);
        Task<List<UserVM>> GetClientsAsync();
        public Task UpdateVoucherStatusesAsync();
        public Task<IEnumerable<GetAllVoucherVM>> SearchVouchersAsync(string input);
        public Task<bool> UpdateVoucherUser(Guid ID, UpdateVC request);
        Task<List<string>> GetVoucherUsersAsync(Guid id);
        Task<bool> IsVoucherCodeExistsAsync(string code);
        public Task<bool> ToggleVoucherStatusAsync(Guid ID, string IDUser);
        public Task<List<VoucherViewModel>> GetVouchersByUserIdWithStatusAsync(string idUser);
        Task<List<GetAllVoucherVM>> FilterVouchersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<GetAllVoucherVM>> GetVouchersByStatus(int isActive);
    }
}

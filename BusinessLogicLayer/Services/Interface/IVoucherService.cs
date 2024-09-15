using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherUser;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IVoucherService
    {
        public Task<List<VoucherVM>> GetAllAsync();
        public Task<List<VoucherVM>> GetAllActiveAsync();
        public Task<VoucherVM> GetByIDAsync(Guid ID);
        public Task<VoucherVM> GetByCodeAsync(string code);
        public Task<List<VoucherVM>> GetVoucherByUser(string IDUser);
        public Task<List<VoucherUserVM>> GetUserInVoucher(Guid ID);
        public Task<List<VoucherVM>> GetVouchersAsync(string? IDUser = null);
    }
}

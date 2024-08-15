using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.VoucherUser;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Implements
{
    public class VoucherUserService : IVoucherUserService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public VoucherUserService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(VoucherUserCreateVM request)
        {
            var obj = new VoucherUser()
            {
                CreateDate = DateTime.Now,
                CreateBy = request.IDUser,
                IDUser = request.IDUser,
                IDVoucher = request.IDVoucher,
                Status = 1
            };

            await _dbcontext.VoucherUser.AddAsync(obj);
            await _dbcontext.SaveChangesAsync();


            return true;
        }

        public async Task<List<VoucherUserVM>> GetAllActiveAsync()
        {
            var activeVoucherUsers = await _dbcontext.VoucherUser
                              .Where(vu => vu.Status != 0)
                              .ToListAsync();

            return _mapper.Map<List<VoucherUserVM>>(activeVoucherUsers);
        }

        public async Task<List<VoucherUserVM>> GetAllAsync()
        {
            var activeVoucherUsers = await _dbcontext.VoucherUser
                                          .ToListAsync();

            return _mapper.Map<List<VoucherUserVM>>(activeVoucherUsers);
        }

        public async Task<VoucherUserVM> GetByIDAsync(Guid IDVoucher, string IDUser)
        {
            var obj = await _dbcontext.VoucherUser
                                      .Where(c => c.IDVoucher == IDVoucher && c.IDUser == IDUser)
                                      .FirstOrDefaultAsync();

            if (obj == null)
            {
                return null; // Hoặc xử lý khi không tìm thấy mục
            }

            var objVM = _mapper.Map<VoucherUserVM>(obj);
            return objVM;
        }

        public async Task<bool> RemoveAsync(Guid IDVoucher, string IDUsers, Guid idUserdelete)
        {
            try
            {
                var voucherUser = await _dbcontext.VoucherUser
                    .FirstOrDefaultAsync(c => c.IDVoucher == IDVoucher && c.IDUser == IDUsers);

                if (voucherUser != null)
                {
                    _dbcontext.VoucherUser.Remove(voucherUser);
                    await _dbcontext.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Guid IDVoucher, string IDUsers, VoucherUserUpdateVM request)
        {
            var voucherUser = await _dbcontext.VoucherUser
                               .FirstOrDefaultAsync(vu => vu.IDVoucher == IDVoucher && vu.IDUser == IDUsers);

            if (voucherUser == null)
            {
                return false; 
            }

            voucherUser.IDVoucher = request.IDVoucher;
            voucherUser.IDUser = request.IDUser;
            voucherUser.Status = request.Status;

            _dbcontext.VoucherUser.Update(voucherUser);
            await _dbcontext.SaveChangesAsync();

            return true;
        }
    }
}

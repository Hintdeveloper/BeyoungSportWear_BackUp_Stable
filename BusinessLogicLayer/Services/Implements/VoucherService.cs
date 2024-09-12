using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherUser;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Implements
{
    public class VoucherService : IVoucherService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public VoucherService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<bool> ActivateVoucherAsync(Guid ID)
        {
            var voucher = await _dbcontext.Voucher.FindAsync(ID);
            if (voucher == null)
            {
                return false;
            }

            voucher.IsActive = StatusVoucher.IsBeginning;
            _dbcontext.Voucher.Update(voucher);
            await _dbcontext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateAsync(VoucherCreateVM request)
        {
            try
            {
                var newVoucher = new Voucher
                {
                    Code = request.Code,
                    Name = request.Name,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Quantity = request.Quantity,
                    Type = request.Type,
                    MinimumAmount = request.MinimumAmount,
                    MaximumAmount = request.MaximumAmount,
                    ReducedValue = request.ReducedValue,
                    IsActive = CalculateVoucherStatus(request.StartDate, request.EndDate),
                    Status = 1,
                    CreateBy = request.CreateBy,
                };
                _dbcontext.Voucher.Add(newVoucher);
                await _dbcontext.SaveChangesAsync();

                foreach (var userid in request.SelectedUser)
                {
                    var voucheruser = new VoucherUser
                    {
                        IDUser = userid,
                        IDVoucher = newVoucher.ID,
                        Status = 1,
                        CreateBy = request.CreateBy
                    };
                    await _dbcontext.VoucherUser.AddAsync(voucheruser);
                }
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private StatusVoucher CalculateVoucherStatus(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.UtcNow;
            if (now < startDate)
            {
                return StatusVoucher.HasntStartedYet;
            }
            else if (now >= startDate && now <= endDate)
            {
                return StatusVoucher.IsBeginning;
            }
            else 
            {
                return StatusVoucher.Finished;
            }
        }
        public async Task<bool> DeactivateVoucherAsync(Guid ID)
        {
            var voucher = await _dbcontext.Voucher.FindAsync(ID);
            if (voucher == null)
            {
                return false; 
            }

            voucher.IsActive = StatusVoucher.HasntStartedYet; 
            _dbcontext.Voucher.Update(voucher);
            await _dbcontext.SaveChangesAsync();

            return true;
        }

        public async Task<List<VoucherVM>> GetAllActiveAsync()
        {
            var obj = await _dbcontext.Voucher
                                    .Where(o => o.Status != 0)
                                    .Include(c => c.VoucherUser)
                       .ToListAsync();

            var mappedvoucher = obj.Select(p =>
            {
                var vouchervm = _mapper.Map<VoucherVM>(p);
                vouchervm.IDUser = p.VoucherUser.Select(pv => pv.IDUser).ToList();
                return vouchervm;
            }).ToList();

            return mappedvoucher;
        }

        public async Task<List<VoucherVM>> GetAllAsync()
        {
            var obj = await _dbcontext.Voucher
                                               .Include(c => c.VoucherUser)
                                  .ToListAsync();

            var mappedvoucher = obj.Select(p =>
            {
                var vouchervm = _mapper.Map<VoucherVM>(p);
                vouchervm.IDUser = p.VoucherUser.Select(pv => pv.IDUser).ToList();
                return vouchervm;
            }).ToList();

            mappedvoucher = mappedvoucher
                               .OrderBy(p => p.Status == 0 ? 1 : 0)
                               .ThenByDescending(p => p.CreateDate)
                               .ToList();

            return mappedvoucher;
        }

        public async Task<VoucherVM> GetByIDAsync(Guid ID)
        {
            var obj = await _dbcontext.Voucher.FindAsync(ID);
            if (obj == null)
            {
                return null;
            }

            return _mapper.Map<VoucherVM>(obj);
        }

        public async Task<List<VoucherUserVM>> GetUserInPromotionAsync(Guid ID)
        {
            var specificPromotion = await _dbcontext.Voucher
                                         .FirstOrDefaultAsync(p => p.ID == ID && p.IsActive == StatusVoucher.IsBeginning);
            if (specificPromotion == null)
            {
                return new List<VoucherUserVM>();
            }

            var variants = await _dbcontext.VoucherUser
                .Where(v => v.IDVoucher == ID && v.Status != 0)
                .ProjectTo<VoucherUserVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return variants;
        }

        public async Task<List<VoucherVM>> GetVoucherByUser(string IDUser)
        {
            var datavoucher = await _dbcontext.VoucherUser
                          .Where(c => c.IDUser == IDUser && c.Status != 0)
                          .Select(c => c.Voucher)
                          .Select(v => new VoucherVM
                          {
                              ID = v.ID,
                              Code = v.Code,
                              Name = v.Name,
                              StartDate = v.StartDate,
                              EndDate = v.EndDate,
                              Quantity = v.Quantity,
                              Type = v.Type,
                              MinimumAmount = v.MinimumAmount,
                              MaximumAmount = v.MaximumAmount,
                              ReducedValue = v.ReducedValue,
                              IsActive = v.IsActive,
                              Status = (DateTime.Now >= v.StartDate && DateTime.Now <= v.EndDate && v.Quantity > 0) ? 1 : 0
                          })
                          .ToListAsync();
            return datavoucher;
        }

        public async Task<List<VoucherVM>> GetVouchersByExpirationDateAsync(DateTime expirationDate)
        {
            var obj = await _dbcontext.Voucher
                                              .Where(v => v.EndDate == expirationDate)
                                              .ToListAsync();

            return _mapper.Map<List<VoucherVM>>(obj);
        }

        public async Task<bool> RemoveAsync(Guid ID, string IDUserdelete)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Voucher.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null)
                    {
                        obj.Status = 0;
                        obj.IsActive = StatusVoucher.Finished;
                        obj.DeleteDate = DateTime.Now;
                        obj.DeleteBy = IDUserdelete;

                        _dbcontext.Voucher.Attach(obj);

                        await _dbcontext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<List<VoucherVM>> SearchVouchersAsync(string keyword)
        {
            var vouchers = await _dbcontext.Voucher
                                               .Where(v => v.Name.Contains(keyword) || v.Code.Contains(keyword))
                                               .ToListAsync();

            return _mapper.Map<List<VoucherVM>>(vouchers);
        }

        public async Task<bool> UpdateAsync(Guid ID, VoucherUpdateVM request)
        {
            var voucher = await _dbcontext.Voucher.FindAsync(ID);
            if (voucher == null)
            {
                return false; 
            }


            voucher.Code = request.Code;
            voucher.Name = request.Name;
            voucher.StartDate = request.StartDate;
            voucher.EndDate = request.EndDate;
            voucher.Quantity = request.Quantity;
            voucher.Type = request.Type;
            voucher.MinimumAmount = request.MinimumAmount;
            voucher.MaximumAmount = request.MaximumAmount;
            voucher.ReducedValue = request.ReducedValue;
            voucher.IsActive = request.IsActive;
            voucher.Status = request.Status;

            var existingLinks = _dbcontext.VoucherUser.Where(vu => vu.IDVoucher == ID);
            _dbcontext.VoucherUser.RemoveRange(existingLinks);

            foreach (var userId in request.SelectedUser)
            {
                var voucherUser = new VoucherUser
                {
                    IDUser = userId,
                    IDVoucher = voucher.ID,
                    Status = 1,
                    CreateBy = request.ModifiedBy
                };
                await _dbcontext.VoucherUser.AddAsync(voucherUser);
            }

            await _dbcontext.SaveChangesAsync();

            return true;
        }
    }
}

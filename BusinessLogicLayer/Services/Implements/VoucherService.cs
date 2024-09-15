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
        public async Task<List<VoucherUserVM>> GetUserInVoucher(Guid ID)
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
                              Status = v.Status
                          })
                          .ToListAsync();
            return datavoucher;
        }
        public async Task<VoucherVM> GetByCodeAsync(string code)
        {
            var voucher = await _dbcontext.Voucher.FirstOrDefaultAsync(c=>c.Code == code);
            if (voucher == null)
            {
                return null; 
            }
            var voucherVM = new VoucherVM
            {
                ID = voucher.ID,
                Code = voucher.Code,
                Name = voucher.Name,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                Quantity = voucher.Quantity,
                Type = voucher.Type,
                MinimumAmount = voucher.MinimumAmount,
                MaximumAmount = voucher.MaximumAmount,
                ReducedValue = voucher.ReducedValue,
                Status = voucher.Status,
                IsActive = voucher.IsActive
            };

            return voucherVM;
        }
        public async Task<List<VoucherVM>> GetVouchersAsync(string? IDUser = null)
        {
            if (string.IsNullOrEmpty(IDUser))
            {
                var publicVouchers = await _dbcontext.Voucher
                    .Where(v => v.Status == 0)  
                    .Include(v => v.VoucherUser)
                    .ToListAsync();

                var mappedPublicVouchers = publicVouchers.Select(v =>
                {
                    var vouchervm = _mapper.Map<VoucherVM>(v);
                    vouchervm.IDUser = v.VoucherUser.Select(pv => pv.IDUser).ToList();
                    return vouchervm;
                }).ToList();

                return mappedPublicVouchers;
            }
            else
            {
                var userVouchers = await _dbcontext.VoucherUser
           .Where(vu => vu.IDUser == IDUser)
           .Select(vu => vu.Voucher)
           .Distinct() // Đảm bảo không có bản sao
           .ToListAsync();

                var voucherIds = userVouchers.Select(v => v.ID).ToList();

                var allUserVouchers = await _dbcontext.Voucher
                    .Where(v => voucherIds.Contains(v.ID) || v.Status == 0) 
                    .Include(v => v.VoucherUser)
                    .ToListAsync();

                var mappedUserVouchers = allUserVouchers.Select(v =>
                {
                    var vouchervm = _mapper.Map<VoucherVM>(v);
                    vouchervm.IDUser = v.VoucherUser != null
                        ? v.VoucherUser.Select(pv => pv.IDUser).ToList()
                        : new List<string>();
                    vouchervm.IsUsed = v.VoucherUser.Any(vu => vu.IDUser == IDUser && vu.Status == 0) ? 1 : 0; 
                    return vouchervm;
                }).ToList();

                return mappedUserVouchers;
            }

        }
    }
}

﻿using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherM;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Implements
{
    public class VoucherMServiece :IVoucherMServiece
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public VoucherMServiece(ApplicationDBContext ApplicationDBContext, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<bool> Create(CreateVoucherVM request)
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
                    IsActive = StatusVoucher.HasntStartedYet,
                    Status = 1,
                    CreateBy = request.CreateBy,
                };
                _dbcontext.Voucher.Add(newVoucher);

                if (request.ApplyToAllUsers)
                {
                    var allUsers = await _dbcontext.Users.Select(u => u.Id).ToListAsync();
                    foreach (var userId in allUsers)
                    {
                        var voucherUser = new VoucherUser
                        {
                            IDUser = userId,
                            IDVoucher = newVoucher.ID,
                            Status = 1,
                            CreateBy = request.CreateBy
                        };
                        await _dbcontext.VoucherUser.AddAsync(voucherUser);
                    }
                }
                else
                {
                    foreach (var userId in request.SelectedUser)
                    {
                        var voucherUser = new VoucherUser
                        {
                            IDUser = userId,
                            IDVoucher = newVoucher.ID,
                            Status = 1,
                            CreateBy = request.CreateBy
                        };
                        await _dbcontext.VoucherUser.AddAsync(voucherUser);
                    }
                }

                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<GetAllVoucherVM>> GetAll()
        {
            var vouchers = await _dbcontext.Voucher
                                           .Include(c => c.VoucherUser)                                          
                                           .OrderBy(p => p.Status == 0 ? 1 : 0)
                                           .ThenByDescending(p => p.CreateDate)
                                           .Select(p => new GetAllVoucherVM
                                           {
                                               ID = p.ID,
                                               CreateDate = p.CreateDate,
                                               Code = p.Code,
                                               Name = p.Name,
                                               StartDate = p.StartDate,
                                               EndDate = p.EndDate,
                                               Quantity = p.Quantity,
                                               Type = p.Type,
                                               MinimumAmount = p.MinimumAmount,
                                               MaximumAmount = p.MaximumAmount,
                                               ReducedValue = p.ReducedValue,
                                               IsActive = p.IsActive,
                                               Status = p.Status,
                                               IDUser = p.VoucherUser.Select(pv => pv.IDUser).ToList()
                                           })
                                           .ToListAsync();

            return vouchers;
        }

        public async Task<List<GetAllVoucherVM>> GetAllAsync()
        {
            var vouchers = await _dbcontext.Voucher
                                           .Include(c => c.VoucherUser)
                                           .Where(p => p.Status == 1)
                                           .OrderBy(p => p.Status == 0 ? 1 : 0)
                                           .ThenByDescending(p => p.CreateDate)
                                           .Select(p => new GetAllVoucherVM
                                           {
                                               ID = p.ID,
                                               CreateDate = p.CreateDate,
                                               Code = p.Code,
                                               Name = p.Name,
                                               StartDate = p.StartDate,
                                               EndDate = p.EndDate,
                                               Quantity = p.Quantity,
                                               Type = p.Type,
                                               MinimumAmount = p.MinimumAmount,
                                               MaximumAmount = p.MaximumAmount,
                                               ReducedValue = p.ReducedValue,
                                               IsActive = p.IsActive,
                                               Status = p.Status,
                                               IDUser = p.VoucherUser.Select(pv => pv.IDUser).ToList()
                                           })
                                           .ToListAsync();

            return vouchers;
        }

        public async Task<GetAllVoucherVM> GetByIDAsync(Guid ID)
        {
            var obj = await _dbcontext.Voucher.FindAsync(ID);
            if (obj == null)
            {
                return null;
            }

            var voucherVM = new GetAllVoucherVM
            {
                ID = obj.ID,
                CreateDate = obj.CreateDate,
                Code = obj.Code,
                Name = obj.Name,
                StartDate = obj.StartDate,
                EndDate = obj.EndDate,
                Quantity = obj.Quantity,
                Type = obj.Type,
                MinimumAmount = obj.MinimumAmount,
                MaximumAmount = obj.MaximumAmount,
                ReducedValue = obj.ReducedValue,
                IsActive = obj.IsActive,
                Status = obj.Status
            };

            return voucherVM;
        }

        public async Task<List<UserVM>> GetClientsAsync()
        {
            var role = await _roleManager.FindByNameAsync("Client");
            if (role == null)
            {
                throw new Exception("Role 'client' not found.");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            var clientList = usersInRole.Where(u => u.Status == 1).Select(u => new UserVM
            {
                Id = u.Id,
                Name = u.FirstAndLastName,
                SDT = u.PhoneNumber,
            }).ToList();

            return clientList;
        }

        public async Task<bool> RemoveAsync(Guid ID, string IDUserdelete)
        {         
                try
                {
                    var obj = await _dbcontext.Voucher.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null)
                    {                      
                        obj.IsActive = StatusVoucher.Finished;
                        obj.DeleteDate = DateTime.Now;
                        obj.DeleteBy = IDUserdelete;
                        _dbcontext.Voucher.Attach(obj);
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

        public async Task<IEnumerable<GetAllVoucherVM>> SearchVouchersAsync(string input)
        {
            var vouchers = await _dbcontext.Voucher
                                           .Include(c => c.VoucherUser)
                                           .Where(p => p.Status == 1)
                                           .OrderBy(p => p.Status == 0 ? 1 : 0)
                                           .ThenByDescending(p => p.CreateDate)
                                           .Select(p => new GetAllVoucherVM
                                           {
                                               ID = p.ID,
                                               CreateDate = p.CreateDate,
                                               Code = p.Code,
                                               Name = p.Name,
                                               StartDate = p.StartDate,
                                               EndDate = p.EndDate,
                                               Quantity = p.Quantity,
                                               Type = p.Type,
                                               MinimumAmount = p.MinimumAmount,
                                               MaximumAmount = p.MaximumAmount,
                                               ReducedValue = p.ReducedValue,
                                               IsActive = p.IsActive,
                                               Status = p.Status,
                                               IDUser = p.VoucherUser.Select(pv => pv.IDUser).ToList()
                                           })
                                           .ToListAsync();

            // Lọc theo mã voucher hoặc tên voucher
            var filteredVouchers = vouchers
                .Where(v => (string.IsNullOrEmpty(input) || v.Code.Contains(input) || v.Name.Contains(input)))
                .ToList();

            return filteredVouchers;
        }

        public async Task<bool> UpdateAsync(Guid ID, UpdateVoucherVM request)
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
            _dbcontext.Voucher.Update(voucher); 
            await _dbcontext.SaveChangesAsync();
            return true;
        }

        public async Task UpdateVoucherStatusesAsync()
        {
            var today = DateTime.Now; 

            // Lấy tất cả voucher từ cơ sở dữ liệu
            var vouchers = await _dbcontext.Voucher.ToListAsync();

            foreach (var voucher in vouchers)
            {

                if (voucher.IsActive != StatusVoucher.Finished)
                {
                    if (voucher.Quantity == 0)
                    {
                        voucher.IsActive = StatusVoucher.Finished;
                    }
                    else if (voucher.StartDate >= today)
                    {
                        voucher.IsActive = StatusVoucher.HasntStartedYet;
                    }
                    else if (voucher.EndDate < today)
                    {
                        voucher.IsActive = StatusVoucher.Finished;
                    }
                    else
                    {
                        voucher.IsActive = StatusVoucher.IsBeginning;
                    }
                }
            }

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<bool> UpdateVoucherUser(Guid id, UpdateVC request)
        {
            var voucher = await _dbcontext.Voucher
         .Include(v => v.VoucherUser)
         .FirstOrDefaultAsync(v => v.ID == id);

            if (voucher == null)
            {
                return false;
            }

            // Cập nhật thông tin voucher
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
            

            if (request.ApplyToAllUsers)
            {
                // Lấy danh sách tất cả khách hàng
                var allUsers = await _dbcontext.Users.ToListAsync();

                var allUserIds = allUsers.Select(u => u.Id).ToHashSet();
                var currentUserIds = voucher.VoucherUser.Select(vu => vu.IDUser).ToHashSet();

                // Thêm tất cả khách hàng chưa liên kết vào voucher
                foreach (var userId in allUserIds.Except(currentUserIds))
                {
                    var voucherUser = new VoucherUser
                    {
                        IDUser = userId,
                        IDVoucher = voucher.ID,
                        Status = 1, // Đặt trạng thái thành 1
                        CreateBy = request.CreateBy
                    };
                    await _dbcontext.VoucherUser.AddAsync(voucherUser);
                }

                // Xóa các khách hàng đã được gỡ liên kết
                foreach (var voucherUser in voucher.VoucherUser.ToList())
                {
                    if (!allUserIds.Contains(voucherUser.IDUser))
                    {
                        _dbcontext.VoucherUser.Remove(voucherUser);
                    }
                }
            }
            else
            {
                var currentUserIds = voucher.VoucherUser.Select(vu => vu.IDUser).ToHashSet();

                // Cập nhật trạng thái của những người dùng hiện tại
                foreach (var voucherUser in voucher.VoucherUser.ToList())
                {
                    if (!request.SelectedUser.Contains(voucherUser.IDUser))
                    {
                        
                        _dbcontext.VoucherUser.Remove(voucherUser);
                    }
                }

                // Thêm mới các người dùng được chọn nhưng chưa có trong voucher
                foreach (var userId in request.SelectedUser)
                {
                    if (!currentUserIds.Contains(userId))
                    {
                        var voucherUser = new VoucherUser
                        {
                            IDUser = userId,
                            IDVoucher = voucher.ID,
                            Status = 1, // Đặt trạng thái thành 1
                            CreateBy = request.CreateBy
                        };
                        await _dbcontext.VoucherUser.AddAsync(voucherUser);
                    }
                }
            }

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
        public async Task<List<string>> GetVoucherUsersAsync(Guid id)
        {
            // Lấy voucher từ DB
            var voucher = await _dbcontext.Voucher
                .Include(v => v.VoucherUser)
                .ThenInclude(vu => vu.ApplicationUser)
                .FirstOrDefaultAsync(v => v.ID == id);

            if (voucher == null)
            {
                // Nếu không tìm thấy voucher, trả về danh sách rỗng
                return new List<string>();
            }

            // Lấy danh sách các ID người dùng đã có voucher
            var userIds = voucher.VoucherUser.Select(vu => vu.IDUser).ToList();
            return userIds;
        }

        public async Task<bool> IsVoucherCodeExistsAsync(string code)
        {
            return await _dbcontext.Voucher.AnyAsync(v => v.Code == code);
        }
    }
}

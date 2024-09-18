using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Services.SignalR;
using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.Voucher;
using BusinessLogicLayer.Viewmodels.VoucherM;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Implements
{
    public class VoucherMServiece : IVoucherMServiece
    {
        private readonly IHubContext<VoucherHub> _hubContext;
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly MailSettings _mailSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public VoucherMServiece(ApplicationDBContext ApplicationDBContext, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHubContext<VoucherHub> hubContext, IOptions<MailSettings> mailSettings)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _hubContext = hubContext;
            _mailSettings = mailSettings.Value;
        }
        public async Task<bool> Create(CreateVoucherVM request)
        {
            try
            {
                var newVoucher = new Voucher
                {
                    Code = request.Code,
                    CreateDate = DateTime.Now,
                    Name = request.Name,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Quantity = request.Quantity,
                    Type = request.Type,
                    MinimumAmount = request.MinimumAmount,
                    MaximumAmount = request.MaximumAmount,
                    ReducedValue = request.ReducedValue,
                    IsActive = StatusVoucher.HasntStartedYet,
                    Status = request.ApplyToAllUsers ? 0 : 1,
                    CreateBy = request.CreateBy,
                };
                _dbcontext.Voucher.Add(newVoucher);
                if (!request.ApplyToAllUsers)
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
                if (!request.ApplyToAllUsers)
                {
                    foreach (var userId in request.SelectedUser)
                    {
                        var user = await _dbcontext.Users.FindAsync(userId);
                        if (user != null)
                        {

                            var emailResponse = await SendConfirmationEmailAsync(
                                user.Email,
                                newVoucher.Code,
                                user.UserName,
                                newVoucher.Name);

                            if (!emailResponse.IsSuccess)
                            {

                            }
                        }
                    }
                }
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
                                           .OrderBy(p => p.IsActive.HasValue ? (int)p.IsActive.Value : int.MaxValue)
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
                Email = u.Email
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
                var oldStatus = voucher.IsActive;
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
                    if (oldStatus != voucher.IsActive)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveVoucherStatusUpdate", voucher.ID, voucher.IsActive);
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

                foreach (var voucherUser in voucher.VoucherUser.ToList())
                {
                    _dbcontext.VoucherUser.Remove(voucherUser);
                }
                voucher.Status = 0;

                // Lấy danh sách tất cả khách hàng
                //var allUsers = await _dbcontext.Users.ToListAsync();

                //var allUserIds = allUsers.Select(u => u.Id).ToHashSet();
                //var currentUserIds = voucher.VoucherUser.Select(vu => vu.IDUser).ToHashSet();

                //// Thêm tất cả khách hàng chưa liên kết vào voucher
                //foreach (var userId in allUserIds.Except(currentUserIds))
                //{
                //    var voucherUser = new VoucherUser
                //    {
                //        IDUser = userId,
                //        IDVoucher = voucher.ID,
                //        Status = 1, // Đặt trạng thái thành 1
                //        CreateBy = request.CreateBy
                //    };
                //    await _dbcontext.VoucherUser.AddAsync(voucherUser);
                //}

                // Xóa các khách hàng đã được gỡ liên kết
                //foreach (var voucherUser in voucher.VoucherUser.ToList())
                //{
                //    if (!allUserIds.Contains(voucherUser.IDUser))
                //    {
                //        _dbcontext.VoucherUser.Remove(voucherUser);
                //    }
                //}
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
        public async Task<bool> ToggleVoucherStatusAsync(Guid ID, string IDUser)
        {
            try
            {
                var voucher = await _dbcontext.Voucher.FirstOrDefaultAsync(c => c.ID == ID);

                if (voucher != null)
                {
                    if (voucher.IsActive == StatusVoucher.IsBeginning || voucher.IsActive == StatusVoucher.HasntStartedYet)
                    {
                        voucher.IsActive = StatusVoucher.Finished;
                    }
                    else if (voucher.IsActive == StatusVoucher.Finished)
                    {
                        voucher.IsActive = voucher.IsActive == StatusVoucher.IsBeginning ? StatusVoucher.HasntStartedYet : StatusVoucher.IsBeginning;
                    }

                    voucher.DeleteDate = DateTime.Now;
                    voucher.DeleteBy = IDUser;

                    _dbcontext.Voucher.Attach(voucher);
                    await _dbcontext.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<VoucherViewModel>> GetVouchersByUserIdWithStatusAsync(string idUser)
        {
            List<VoucherViewModel> voucherList;

            if (idUser == "1")
            {
                // Trường hợp không có IDUser, trả về các voucher có status = 0 và IsActive = IsBeginning
                voucherList = await _dbcontext.Voucher
                                              .Where(vu => vu.Status == 0
                                                        && vu.IsActive == StatusVoucher.IsBeginning)
                                              .Select(vu => new VoucherViewModel
                                              {
                                                  ID = vu.ID,
                                                  MinimumAmount = vu.MinimumAmount,
                                                  MaximumAmount = vu.MaximumAmount,
                                                  Code = vu.Code,
                                                  Name = vu.Name,
                                                  StartDate = vu.StartDate,
                                                  EndDate = vu.EndDate,
                                                  Quantity = vu.Quantity,
                                                  Type = vu.Type,
                                                  ReducedValue = vu.ReducedValue,
                                                  IsActive = vu.IsActive,
                                                  Status = vu.Status,
                                              })
                                              .ToListAsync();
            }
            else
            {
                var connectedVouchers = await _dbcontext.VoucherUser
                                                .Where(vu => vu.IDUser == idUser
                                                          //&& vu.Status == 0
                                                          && vu.Voucher.Status == 1
                                                          && vu.Voucher.IsActive == StatusVoucher.IsBeginning)
                                                .Select(vu => new VoucherViewModel
                                                {
                                                    ID = vu.IDVoucher,
                                                    IDUser = vu.IDUser,
                                                    MinimumAmount = vu.Voucher.MinimumAmount,
                                                    MaximumAmount = vu.Voucher.MaximumAmount,
                                                    Type = vu.Voucher.Type,
                                                    Code = vu.Voucher.Code,
                                                    Name = vu.Voucher.Name,
                                                    StartDate = vu.Voucher.StartDate,
                                                    EndDate = vu.Voucher.EndDate,
                                                    Quantity = vu.Voucher.Quantity,
                                                    ReducedValue = vu.Voucher.ReducedValue,
                                                    IsActive = vu.Voucher.IsActive,
                                                    Status = vu.Status,
                                                })
                                                .ToListAsync();


                var unconnectedVouchers = await _dbcontext.Voucher
                                                          .Where(v => v.Status == 0
                                                                    && v.IsActive == StatusVoucher.IsBeginning
                                                                    && !_dbcontext.VoucherUser.Any(vu => vu.IDUser == idUser && vu.IDVoucher == v.ID))
                                                          .Select(v => new VoucherViewModel
                                                          {
                                                              ID = v.ID,
                                                              MinimumAmount = v.MinimumAmount,
                                                              MaximumAmount = v.MaximumAmount,
                                                              Type = v.Type,
                                                              Code = v.Code,
                                                              Name = v.Name,
                                                              StartDate = v.StartDate,
                                                              EndDate = v.EndDate,
                                                              Quantity = v.Quantity,
                                                              ReducedValue = v.ReducedValue,
                                                              IsActive = v.IsActive,
                                                              Status = v.Status,
                                                          })
                                                          .ToListAsync();

                voucherList = connectedVouchers.Concat(unconnectedVouchers).ToList();
            }

            return voucherList;
        }


        public async Task<List<GetAllVoucherVM>> FilterVouchersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var vouchers = await _dbcontext.Voucher
                                           .Include(c => c.VoucherUser)
                                           .Where(v => v.StartDate >= startDate && v.EndDate <= endDate) // Điều kiện lọc theo ngày
                                           .OrderBy(v => v.IsActive.HasValue ? (int)v.IsActive.Value : int.MaxValue)
                                           .ThenByDescending(v => v.CreateDate)
                                           .Select(v => new GetAllVoucherVM
                                           {
                                               ID = v.ID,
                                               CreateDate = v.CreateDate,
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
                                               Status = v.Status,
                                               IDUser = v.VoucherUser.Select(pv => pv.IDUser).ToList()
                                           })
                                           .ToListAsync();

            return vouchers;
        }
        public async Task<List<GetAllVoucherVM>> GetVouchersByStatus(int isActive)
        {
            var vouchers = await _dbcontext.Voucher
                .Where(v => v.IsActive.HasValue && (int)v.IsActive.Value == isActive)
                .OrderBy(v => v.CreateDate)
                .Select(v => new GetAllVoucherVM
                {
                    ID = v.ID,
                    CreateDate = v.CreateDate,
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
                    Status = v.Status,
                    IDUser = v.VoucherUser.Select(vu => vu.IDUser).ToList()
                })
                .ToListAsync();

            return vouchers;
        }
        private async Task<Response> SendConfirmationEmailAsync(string email, string keycode, string name, string Namecode)
        {
            string body = $@"
                    <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                line-height: 1.6;
                                color: #333;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #ddd;
                                border-radius: 10px;
                                background-color: #f9f9f9;
                            }}
                            .header {{
                                text-align: center;
                                padding-bottom: 20px;
                            }}
                            .header img {{
                                max-width: 100px;
                            }}
                            .content {{
                                padding: 20px;
                                background-color: #fff;
                                border-radius: 10px;
                            }}
                            .order-info {{
                                margin-top: 20px;
                                padding: 10px;
                                background-color: #eee;
                                border-radius: 5px;
                            }}
                            .product-item {{
                                margin-top: 10px;
                                padding: 10px;
                                border-bottom: 1px solid #ddd;
                                display: flex;
                                align-items: center;
                            }}
                            .product-item img {{
                                max-width: 130px;
                                margin-right: 10px;
                            }}
                            .product-details {{
                                flex: 1;
                            }}
                            .footer {{
                                text-align: center;
                                font-size: 12px;
                                color: #999;
                                margin-top: 20px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <img src='https://res.cloudinary.com/dqcxurnpa/image/upload/v1723407933/BeyoungSportWear/ImageProduct/Options/l1zudx2ihhv6noe0ecga.webp' alt='Company Logo' />
                                <h2>Chào mừng bạn đến với trang web bán quần áo thể thao Beyoung Sport Wear!</h2>
                            </div>
                            <div class='content'>
                                <p>Kính gửi quý khách hàng {name},</p>
                                <p><strong>Shop Beyoung Sport Wear</strong> xin chân thành cảm ơn quý khách đã luôn đồng hành cùng chúng tôi. Nhân dịp này, chúng tôi xin gửi tặng quý khách voucher <strong>{Namecode}</strong> với mã voucher <strong>{keycode}</strong> như một lời tri ân.</p>
                                <p>Cảm ơn quý khách và rất mong quý khách tiếp tục sử dụng dịch vụ của shop chúng tôi trong tương lai</p>
                                <p>Trân trọng!</p>
                                ";

            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                    Subject = "Thư thông báo khách hàng được nhận voucher",
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(email));

                using (SmtpClient smtp = new SmtpClient(_mailSettings.Host, _mailSettings.Port))
                {
                    smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail);
                }

                return new Response
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Email xác nhận đã được gửi."
                };
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return new Response
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = $"Lỗi khi gửi email xác nhận: {ex.Message}"
                };
            }
        }
    }
}

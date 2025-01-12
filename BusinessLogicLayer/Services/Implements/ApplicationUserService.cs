﻿using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.ApplicationUser;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using BusinessLogicLayer.Viewmodels.Address;
using Org.BouncyCastle.Utilities.Net;
namespace BusinessLogicLayer.Services.Implements
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly MailSettings _mailSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;

        public ApplicationUserService(ApplicationDBContext ApplicationDBContext,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IOptions<MailSettings> mailSettings,
            UserManager<ApplicationUser> userManager,
                           IHttpContextAccessor httpContextAccessor,
                           IConfiguration IConfiguration,
                           Cloudinary cloudinary)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = ApplicationDBContext;
            _mailSettings = mailSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _configuration = IConfiguration;
            _cloudinary = cloudinary;
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512Signature)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task SaveUserLoginAsync(ApplicationUser user, string loginProvider, string providerKey, string displayName)
        {
            var userLoginInfo = new UserLoginInfo(loginProvider, providerKey, displayName);
            await _userManager.AddLoginAsync(user, userLoginInfo);
        }
        private async Task SaveUserClaimsAsync(ApplicationUser user, List<Claim> claims)
        {
            foreach (var claim in claims)
            {
                await _userManager.AddClaimAsync(user, claim);
            }
        }
        private async Task SaveJwtTokenToUserAsync(ApplicationUser user, string token)
        {
            await _userManager.SetAuthenticationTokenAsync(user, _configuration["JWT:Issuer"], "JWT", token);
        }
        private async Task<Response> SendConfirmationEmailAsync(string email, Uri callbackUri)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                    Subject = "Xác nhận địa chỉ email",
                    Body = $"Vui lòng xác nhận địa chỉ email của bạn bằng cách nhấp vào <a href='{callbackUri}'>đây</a>.",
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
        public async Task<List<UserDataVM>> GetAllActiveInformationAsync()
        {
            var users = await _dbContext.ApplicationUser
                .Where(c => c.Status == 1)
                            .ToListAsync();

            var userDataList = users.Select(u => new UserDataVM
            {
                ID = u.Id,
                Username = u.UserName,
                Email = u.Email,
                FirstAndLastName = u.FirstAndLastName,
                PhoneNumber = u.PhoneNumber,
                Images = u.Images,
                Status = u.Status,
            }).ToList();

            return userDataList;
        }
        public async Task<List<UserDataVM>> GetAllInformationAsync()
        {
            var userRoles = await (from user in _dbContext.ApplicationUser
                                   join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                                   join role in _dbContext.Roles on userRole.RoleId equals role.Id
                                   select new { user, role.Name }).ToListAsync();

            var userDataList = userRoles.Select(ur => new UserDataVM
            {
                ID = ur.user.Id,
                Username = ur.user.UserName,
                Email = ur.user.Email,
                Images = ur.user.Images,
                FirstAndLastName = ur.user.FirstAndLastName,
                DateOfBirth = ur.user.DateOfBirth.Date,
                PhoneNumber = ur.user.PhoneNumber,
                Status = ur.user.Status,
                RoleName = ur.Name
            }).ToList();

            return userDataList;
        }
        public async Task<UserDataVM> GetInformationByID(string ID)
        {
            var user = await _dbContext.ApplicationUser
       .Include(u => u.Addresss) // Load địa chỉ
       .Where(u => u.Id == ID)
       .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            // Chuyển đổi từng địa chỉ từ Address sang AddressVM
            var addressVMs = user.Addresss.Select(address => new AddressVM
            {
                ID = address.ID,
                City = address.City,
                DistrictCounty = address.DistrictCounty,
                Commune = address.Commune,
                SpecificAddress = address.SpecificAddress
            }).ToList();

            var userVM = new UserDataVM
            {
                ID = user.Id,
                FirstAndLastName = user.FirstAndLastName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth.Date,
                Email = user.Email,
                Images = user.Images,
                Username = user.UserName,
                AddressVMs = addressVMs,
                Status = user.Status,
            };

            return userVM;
        }
        public async Task<Response> Login(UserLoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.PassWord))
            {
                return new Response { IsSuccess = false, StatusCode = 400, Message = "Username and password must be provided." };
            }

            try
            {

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null || !(await _userManager.CheckPasswordAsync(user, model.PassWord)))
                {
                    return new Response { IsSuccess = false, StatusCode = 401, Message = "Invalid credentials." };
                }
                if (user.Status == 0)
                {
                    return new Response { IsSuccess = false, StatusCode = 403, Message = "Đã bị khóa" };
                }
                await _userManager.RemoveAuthenticationTokenAsync(user, _configuration["JWT:Issuer"], "JWT");
                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var login in logins)
                {
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }
                var claims = await _userManager.GetClaimsAsync(user);
                foreach (var claim in claims)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                var tokenString = await GenerateJwtToken(user);

                await SaveJwtTokenToUserAsync(user, tokenString);

                await SaveUserLoginAsync(user, "XXXXXXXXXX", "ProviderKey", "UserDisplayName");

                var userClaims = new List<Claim>();
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                await SaveUserClaimsAsync(user, userClaims);

                var roles = await _userManager.GetRolesAsync(user);

                return new Response
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Authentication successful.",
                    Token = tokenString,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in Login: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new Response { IsSuccess = false, StatusCode = 500, Message = "Internal server error." };
            }
        }
        public async Task<Response> RegisterAsync(RegisterUser registerUser, string role)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerUser.Email);
                if (existingUser != null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "This email is already in use."
                    };
                }

                existingUser = await _userManager.FindByNameAsync(registerUser.Username);
                if (existingUser != null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "This username is already taken."
                    };
                }

                if (registerUser.Password != registerUser.ConfirmPassword)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "The password and confirmation password do not match."
                    };
                }

                var newUser = new ApplicationUser
                {
                    UserName = registerUser.Username,
                    Email = registerUser.Email,
                    FirstAndLastName = registerUser.FirstAndLastName,
                    PhoneNumber = registerUser.PhoneNumber,
                    Gender = registerUser.Gender,
                    DateOfBirth = registerUser.DateOfBirth,
                    Status = 1,
                    EmailConfirmed = false,
                };

                if (registerUser.Images != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(registerUser.Images.FileName, registerUser.Images.OpenReadStream()),
                        Folder = "BeyoungSportWear",
                        Overwrite = true
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    newUser.Images = uploadResult.SecureUri.AbsoluteUri;
                }

                var result = await _userManager.CreateAsync(newUser, registerUser.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 500,
                        Message = "User creation failed."
                    };
                }

                var cart = new Cart
                {
                    ID = Guid.NewGuid().ToString(),
                    IDUser = newUser.Id,
                    Status = 1,
                };
                await _dbContext.Cart.AddAsync(cart);

                var addresss = new Address
                {
                    ID = Guid.NewGuid(),
                    CreateBy = newUser.Id,
                    CreateDate = DateTime.Now,
                    IDUser = newUser.Id,
                    FirstAndLastName = registerUser.FirstAndLastName,
                    PhoneNumber = registerUser.PhoneNumber,
                    Gmail = registerUser.Email,
                    City = registerUser.City,
                    DistrictCounty = registerUser.DistrictCounty,
                    Commune = registerUser.Commune,
                    SpecificAddress = registerUser.SpecificAddress,
                    Status = 1,
                };
                await _dbContext.Address.AddAsync(addresss);

                await _dbContext.SaveChangesAsync();

                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(newUser, role);

                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    var host = _httpContextAccessor.HttpContext.Request.Host;

                    var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{host}/Account/ConfirmEmail?userId={newUser.Id}&code={emailConfirmationToken}";

                    var callbackUri = new Uri(callbackUrl);
                    await SendConfirmationEmailAsync(newUser.Email, callbackUri);

                    await transaction.CommitAsync();

                    return new Response
                    {
                        IsSuccess = true,
                        StatusCode = 201,
                        Message = "Register successfully! Please check your email for confirmation."
                    };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 500,
                        Message = "Register failed, something went wrong!"
                    };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new Response
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "An error occurred while saving the entity changes. See the inner exception for details.",
                };
            }
        }
        public async Task<bool> SetStatus(Guid ID)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbContext.ApplicationUser.FirstOrDefaultAsync(c => c.Id == ID.ToString());

                    if (obj != null && obj.Status==1)
                    {
                        obj.Status = 0;
                        _dbContext.ApplicationUser.Attach(obj);
                        await _dbContext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else if(obj != null && obj.Status == 0)
                    {
                        obj.Status = 1;
                        _dbContext.ApplicationUser.Attach(obj);
                        await _dbContext.SaveChangesAsync();


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
        public async Task<bool> UpdateUserAsync(string ID, UserUpdateVM userUpdateVM)
        {
            var user = await _userManager.FindByIdAsync(ID);
            if (user == null)
            {
                return false;
            }

            user.FirstAndLastName = userUpdateVM.FirstAndLastName ?? user.FirstAndLastName;
            user.Email = userUpdateVM.Email ?? user.Email;
            user.PhoneNumber = userUpdateVM.PhoneNumber ?? user.PhoneNumber;
            user.Gender = userUpdateVM.Gender ?? user.Gender;
            user.DateOfBirth = userUpdateVM.DateOfBirth ?? user.DateOfBirth;

            // Cập nhật thông tin địa chỉ nếu có thay đổi
            if (userUpdateVM.AddressUpdateVM != null)
            {
                // Tìm địa chỉ của người dùng hiện tại
                var address = user.Addresss?.FirstOrDefault(a =>
                    a.City == userUpdateVM.AddressUpdateVM.City &&
                    a.DistrictCounty == userUpdateVM.AddressUpdateVM.DistrictCounty &&
                    a.Commune == userUpdateVM.AddressUpdateVM.Commune); 

                if (address == null)
                {
                    // Nếu người dùng chưa có địa chỉ, tạo địa chỉ mới
                    address = new Address
                    {
                        IDUser = user.Id,
                        FirstAndLastName = userUpdateVM.FirstAndLastName ?? user.FirstAndLastName,
                        PhoneNumber = userUpdateVM.PhoneNumber ?? user.PhoneNumber,
                        Gmail = userUpdateVM.Email ?? user.Email,
                        City = userUpdateVM.AddressUpdateVM.City,
                        DistrictCounty = userUpdateVM.AddressUpdateVM.DistrictCounty,
                        Commune = userUpdateVM.AddressUpdateVM.Commune,
                        SpecificAddress = userUpdateVM.AddressUpdateVM.SpecificAddress,
                        Status = 1,
                        ModifiedBy = user.Id,
                        CreateBy = user.Id,
                        ID = Guid.NewGuid() // Sử dụng một giá trị GUID mới để đảm bảo khóa chính là duy nhất
                    };
                    _dbContext.Address.Add(address);
                }
                else
                {
                    // Nếu người dùng đã có địa chỉ, cập nhật địa chỉ hiện tại
                    address.City = userUpdateVM.AddressUpdateVM.City ?? address.City;
                    address.DistrictCounty = userUpdateVM.AddressUpdateVM.DistrictCounty ?? address.DistrictCounty;
                    address.Commune = userUpdateVM.AddressUpdateVM.Commune ?? address.Commune;
                    address.SpecificAddress = userUpdateVM.AddressUpdateVM.SpecificAddress ?? address.SpecificAddress;
                    address.Status = 1; // Cập nhật trạng thái nếu cần
                    address.ModifiedBy = user.Id;
                }
            }
            if (userUpdateVM.Images != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(userUpdateVM.Images.FileName, userUpdateVM.Images.OpenReadStream()),
                    Folder = "BeyoungSportWear",
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                user.Images = uploadResult.SecureUri.AbsoluteUri;
            }

            var result = await _userManager.UpdateAsync(user);
            await _dbContext.SaveChangesAsync();
            return result.Succeeded;
        }
        private async Task<List<UserDataVM>> GetUsersByCriteriaAsync(Func<ApplicationUser, bool> criteria)
        {
            var users = await Task.Run(() => _userManager.Users.Where(criteria).ToList());
            var userDataList = new List<UserDataVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault(); 

                userDataList.Add(new UserDataVM
                {
                    ID = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Images = user.Images,
                    FirstAndLastName = user.FirstAndLastName,
                    DateOfBirth = user.DateOfBirth.Date,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = roleName,
                    Status = user.Status,
                });
            }

            return userDataList;
        }

        public async Task<List<UserDataVM>> GetUsersByEmailAsync(string email)
        {
            return await GetUsersByCriteriaAsync(u => u.Email.Contains(email));
        }

        public async Task<List<UserDataVM>> GetUsersByPhoneNumberAsync(string phoneNumber)
        {
            return await GetUsersByCriteriaAsync(u => u.PhoneNumber.Contains(phoneNumber));
        }

        public async Task<List<UserDataVM>> GetUsersByStatusAsync(int status)
        {
            return await GetUsersByCriteriaAsync(u => u.Status == status);
        }

        public async Task<List<UserDataVM>> GetUsersByNameAsync(string name)
        {
            return await GetUsersByCriteriaAsync(u => u.FirstAndLastName.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

    }
}

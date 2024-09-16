using AutoMapper;
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
using Microsoft.AspNetCore.Hosting;
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
        private async Task<Response> SendConfirmationEmailAsync(string email, string name, Uri callbackUri, string userPassword)
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
                <h2>Chào mừng bạn đến với Beyoung Sport Wear!</h2>
            </div>
            <div class='content'>
                <p>Chào {name},</p>
                <p>Chúng tôi vui mừng thông báo rằng bạn đã đăng ký tài khoản thành công trên trang web của chúng tôi.</p>
                <p>Để đăng nhập vào tài khoản của bạn, vui lòng nhấp vào <a href='{callbackUri}'>đây</a> và sử dụng mật khẩu của bạn là: <strong>{userPassword}</strong></p>
                <p>Chúng tôi rất mong được phục vụ bạn!</p>
            </div>
            <div class='footer'>
                <p>Beyoung Sport Wear - Địa chỉ liên hệ: [địa chỉ]</p>
                <p>© {DateTime.Now.Year} Beyoung Sport Wear. All rights reserved.</p>
            </div>
        </div>
    </body>
    </html>";


            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                    Subject = "Thư thông báo đăng ký tài khoản thành công",
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
                Gender = u.Gender,
                PhoneNumber = u.PhoneNumber,
                Images = u.Images,
                Status = u.Status,
            }).OrderByDescending(u => u.JoinDate).ToList();

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
                JoinDate = ur.user.JoinDate,
                Username = ur.user.UserName,
                Email = ur.user.Email,
                Images = ur.user.Images,
                FirstAndLastName = ur.user.FirstAndLastName,
                Gender = ur.user.Gender,
                DateOfBirth = ur.user.DateOfBirth.Date,
                PhoneNumber = ur.user.PhoneNumber,
                Status = ur.user.Status,
                RoleName = ur.Name
            }).OrderByDescending(u => u.JoinDate).ToList();

            return userDataList;
        }
        public async Task<UserDataVM> GetInformationByID(string ID)
        {
            var user = await _dbContext.ApplicationUser
               .Include(u => u.Addresss)
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
                SpecificAddress = address.SpecificAddress,
                IsDefault = address.IsDefault
            }).ToList();

            var userVM = new UserDataVM
            {
                ID = user.Id,
                FirstAndLastName = user.FirstAndLastName,
                Gender = user.Gender,
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
        private static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            Random random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<Response> RegisterWithRandomPasswordAsync(RegisterOnly registerOnly, string role)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(registerOnly.Email);
                if (existingUserByEmail != null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Email đã có người đăng ký"
                    };
                }

                var existingUserByUsername = await _userManager.FindByNameAsync(registerOnly.Username);
                if (existingUserByUsername != null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Tên tài khoản này đã có người đăng ký"
                    };
                }
                var existingUserByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerOnly.PhoneNumber);
                if (existingUserByPhone != null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Số điện thoại này đã được sử dụng."
                    };
                }
                string password = GenerateRandomPassword(12);

                var newUser = new ApplicationUser
                {
                    UserName = registerOnly.Username,
                    Email = registerOnly.Email,
                    FirstAndLastName = registerOnly.FirstAndLastName,
                    PhoneNumber = registerOnly.PhoneNumber,
                    Gender = registerOnly.Gender,
                    EmailConfirmed = false,
                    Status = 1,
                    JoinDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(newUser, password);
                var cart = new Cart
                {
                    ID = newUser.Id,
                    IDUser = newUser.Id,
                    Description = "Giỏ hàng mặc định",
                    Status = 1,
                };
                await _dbContext.Cart.AddAsync(cart);
                await _dbContext.SaveChangesAsync();

                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(newUser, role);
                    }
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    var host = _httpContextAccessor.HttpContext.Request.Host;
                    var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{host}/Login";

                    string emailBody = $@"
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
                                <p>Chào {registerOnly.FirstAndLastName},</p>
                                <p>Bạn đã đăng ký tài khoản thành công tại kênh bán hàng của chúng tôi. Bạn hãy đăng nhập tại <a href='{callbackUrl}'>đây.</a> với mật khẩu là: {password}</p>
                                ";

                    await SendEmailAsync(registerOnly.Email, "Đăng ký tài khoản thành công", emailBody);

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
                        Message = "User creation failed."
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
                    Message = $"An error occurred while saving the entity changes: {ex.Message}",
                };
            }
        }
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
            var toAddress = new MailAddress(toEmail);
            var fromPassword = _mailSettings.Password;
            var smtpHost = _mailSettings.Host;
            var smtpPort = _mailSettings.Port;

            var smtp = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            try
            {
                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
        public async Task<Response> Login(UserLoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.PassWord))
            {
                return new Response { IsSuccess = false, StatusCode = 400, Message = "Tên đăng nhập và mật khẩu không được để trống." };
            }

            try
            {

                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null || !(await _userManager.CheckPasswordAsync(user, model.PassWord)))
                {
                    return new Response { IsSuccess = false, StatusCode = 401, Message = "Thông tin đăng nhập không hợp lệ." };
                }
                if (user.Status == 0)
                {
                    return new Response { IsSuccess = false, StatusCode = 403, Message = "Tài khoản đã bị khóa." };
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
                    Message = "Đăng nhập thành công.",
                    Token = tokenString,
                    Roles = roles.ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in Login: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new Response { IsSuccess = false, StatusCode = 500, Message = "Lỗi hệ thống." };
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
                        Message = "Email đã có người đăng ký"
                    };
                }

                existingUser = await _userManager.FindByNameAsync(registerUser.Username);
                if (existingUser != null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Tên tài khoản này đã có người đăng ký"
                    };
                }
                existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerUser.PhoneNumber);
                if (existingUser != null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Số điện thoại này đã được sử dụng."
                    };
                }
                if (registerUser.Password != registerUser.ConfirmPassword)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Mật khẩu nhập lại không trùng với mật khẩu"
                    };
                }
                int userAge = DateTime.Now.Year - registerUser.DateOfBirth.Year;
                if (userAge < 18 || userAge > 65 && role == "Staff")
                {
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 400,
                        Message = "Tuổi cần cao hơn 18 hoặc nhỏ hơn 65"
                    };

                }

                var newUser = new ApplicationUser
                {
                    JoinDate = DateTime.Now,
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
                else
                {
                    var defaultImagePath = Path.Combine("..", "ExternalInterfaceLayer", "wwwroot", "nonameAva.png");

                    using (var fileStream = new FileStream(defaultImagePath, FileMode.Open))
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription("nonameAva.png", fileStream),
                            Folder = "BeyoungSportWear",
                            Overwrite = true
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        newUser.Images = uploadResult.SecureUri.AbsoluteUri;
                    }
                }

                var result = await _userManager.CreateAsync(newUser, registerUser.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();

                    var errors = result.Errors.Select(e => TranslateErrorToVietnamese(e.Description)).ToList();

                    string errorMessage = "Tạo người dùng thất bại. Chi tiết lỗi: " + string.Join(", ", errors);

                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 500,
                        Message = errorMessage
                    };
                }

                var cart = new Cart
                {
                    ID = Guid.NewGuid().ToString(),
                    IDUser = newUser.Id,
                    Status = 1,
                };
                await _dbContext.Cart.AddAsync(cart);
                await _dbContext.SaveChangesAsync();

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
                    IsDefault = true,
                    Status = 1,
                };
                await _dbContext.Address.AddAsync(addresss);

                await _dbContext.SaveChangesAsync();

                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(newUser, role);

                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                    var host = _httpContextAccessor.HttpContext.Request.Host;

                    var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{host}/Login";

                    var callbackUri = new Uri(callbackUrl);
                    await SendConfirmationEmailAsync(newUser.Email, newUser.FirstAndLastName, callbackUri, registerUser.Password);

                    await transaction.CommitAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "Đăng ký thành công, vui lòng kiểm tra email để xác nhận."
                    };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new Response
                    {
                        IsSuccess = false,
                        StatusCode = 500,
                        Message = "Có lỗi xảy ra trong quá trình lưu dữ liệu, vui lòng kiểm tra lại."
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
                    Message = "Đã xảy ra lỗi khi lưu các thay đổi thực thể. Xem ngoại lệ bên trong để biết chi tiết.",
                };
            }
        }
        private string TranslateErrorToVietnamese(string error)
        {
            var errorDictionary = new Dictionary<string, string>
                {
                    { "Passwords must have at least one uppercase ('A'-'Z').", "Mật khẩu phải có ít nhất một chữ cái viết hoa ('A'-'Z')." },
                    { "Passwords must have at least one lowercase ('a'-'z').", "Mật khẩu phải có ít nhất một chữ cái thường ('a'-'z')." },
                    { "Passwords must have at least one digit ('0'-'9').", "Mật khẩu phải có ít nhất một chữ số ('0'-'9')." },
                    { "Passwords must have at least one non-alphanumeric character.", "Mật khẩu phải có ít nhất một ký tự đặc biệt." },
                    { "Password too short.", "Mật khẩu quá ngắn." },
                    { "Email already taken.", "Email đã có người đăng ký." },
                    { "User name already taken.", "Tên người dùng đã tồn tại." }
                };

            return errorDictionary.ContainsKey(error) ? errorDictionary[error] : error;
        }
        public async Task<bool> SetStatus(Guid ID)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbContext.ApplicationUser.FirstOrDefaultAsync(c => c.Id == ID.ToString());

                    if (obj != null && obj.Status == 1)
                    {
                        obj.Status = 0;
                        _dbContext.ApplicationUser.Attach(obj);
                        await _dbContext.SaveChangesAsync();


                        transaction.Commit();
                        return true;
                    }
                    else if (obj != null && obj.Status == 0)
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

                var addresses = _dbContext.Address
                .Where(a => a.IDUser == user.Id)
                .Select(a => new AddressVM
                {
                    ID = a.ID,
                    IDUser = a.IDUser,
                    City = a.City,
                    PhoneNumber = a.PhoneNumber,
                    Gmail = a.Gmail,
                    FirstAndLastName = a.FirstAndLastName,
                    DistrictCounty = a.DistrictCounty,
                    Commune = a.Commune,
                    SpecificAddress = a.SpecificAddress,
                    IsDefault = a.IsDefault,
                    Status = a.Status
                })
                .ToList();

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
                    AddressVMs = addresses
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

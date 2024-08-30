using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IApplicationUserService
    {
        Task<List<UserDataVM>> GetAllInformationAsync();
        Task<List<UserDataVM>> GetAllActiveInformationAsync();
        Task<UserDataVM> GetInformationByID(string ID);
        Task<bool> SetStatus(Guid ID);
        Task<Response> RegisterAsync(RegisterUser registerUser, string role);
        Task<Response> RegisterWithRandomPasswordAsync(RegisterOnly registerUser, string role);
        public Task<bool> UpdateUserAsync(string ID, UserUpdateVM userUpdateVM);
        public Task<Response> Login(UserLoginModel model);
        Task<List<UserDataVM>> GetUsersByEmailAsync(string email);
        Task<List<UserDataVM>> GetUsersByPhoneNumberAsync(string phoneNumber);
        Task<List<UserDataVM>> GetUsersByStatusAsync(int status);
        Task<List<UserDataVM>> GetUsersByNameAsync(string name);
    }
}

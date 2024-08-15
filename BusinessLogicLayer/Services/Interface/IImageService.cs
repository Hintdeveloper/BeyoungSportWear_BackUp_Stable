using BusinessLogicLayer.Viewmodels.Brand;
using BusinessLogicLayer.Viewmodels.Image;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
        public Task<List<ImageVM>> GetAllAsync();
        public Task<List<ImageVM>> GetAllActiveAsync();
        public Task<ImageVM> GetByIDAsync(Guid ID);
        public Task<ImageCreateVM> CreateAsync(ImageCreateVM request, Cloudinary cloudinary);
        public Task<bool> RemoveAsync(Guid ID, string IDUserDelete);
        public Task<bool> UpdateAsync(Guid ID, ImageUpdateVM request);
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Viewmodels.Image
{
    public class ImageCreateVM
    {
        public string CreateBy { get; set; } = null!;
        public Guid IDProductDetails { get; set; }
        public List<IFormFile> Path { get; set; } = null!;
        public string? Hash { get; set; }
        public int Status { get; set; }
        public List<string> UploadedImageUrls { get; set; } = new();
    }
}

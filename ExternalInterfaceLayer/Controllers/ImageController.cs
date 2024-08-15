using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Image;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;

namespace YourProject.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public ImageController(IImageService imageService, IMapper mapper, Cloudinary Cloudinary)
        {
            _imageService = imageService;
            _mapper = mapper;
            _cloudinary = Cloudinary;
        }
        [HttpPost("upload_single")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            try
            {
                var imageUrl = await _imageService.UploadImageAsync(imageFile);
                return Ok(new { imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        // POST api/images
        [HttpPost]
        [Route("upload_images")]
        public async Task<IActionResult> Create([FromForm]ImageCreateVM request)
        {
            try
            {
                var updatedRequest = await _imageService.CreateAsync(request, _cloudinary);
                if (updatedRequest != null)
                {
                    return Ok(request);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to upload image or save to database.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // GET api/images
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var images = await _imageService.GetAllAsync();
                var imageVMs = _mapper.Map<List<ImageVM>>(images);
                return Ok(imageVMs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // GET api/images/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var image = await _imageService.GetByIDAsync(id);
                if (image == null)
                {
                    return NotFound();
                }

                var imageVM = _mapper.Map<ImageVM>(image);
                return Ok(imageVM);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // DELETE api/images/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _imageService.RemoveAsync(id, "UserIDToRemoveImage"); // Thay thế "UserIDToRemoveImage" bằng thông tin thực tế của người dùng
                if (result)
                {
                    return Ok("Image deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}

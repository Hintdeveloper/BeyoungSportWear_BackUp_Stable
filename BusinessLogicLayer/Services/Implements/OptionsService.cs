using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Options;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayer.Viewmodels;

namespace BusinessLogicLayer.Services.Implements
{
    public class OptionsService : IOptionsService
    {
        private readonly Cloudinary _cloudinary;

        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public OptionsService(ApplicationDBContext ApplicationDBContext, IMapper mapper, Cloudinary Cloudinary)
        {
            _dbcontext = ApplicationDBContext;
            _cloudinary = Cloudinary;
            _mapper = mapper;
        }
        public async Task<Guid> EnsureSize(string sizeName, string CreateBy)
        {
            var size = await _dbcontext.Sizes
                .FirstOrDefaultAsync(s => s.Name.ToLower() == sizeName.ToLower());

            if (size == null)
            {
                size = new Sizes { ID = Guid.NewGuid(), Name = sizeName, CreateBy = CreateBy, Status = 1 };
                await _dbcontext.Sizes.AddAsync(size);
                await _dbcontext.SaveChangesAsync();
            }

            return size.ID;
        }
        public async Task<Guid> EnsureColor(string colorName, string CreateBy)
        {
            var color = await _dbcontext.Colors
                .FirstOrDefaultAsync(c => c.Name.ToLower() == colorName.ToLower());

            if (color == null)
            {
                color = new Colors { ID = Guid.NewGuid(), Name = colorName, CreateBy = CreateBy, Status = 1 };
                await _dbcontext.Colors.AddAsync(color);
                await _dbcontext.SaveChangesAsync();
            }

            return color.ID;
        }
        public async Task<bool> CreateAsync(OptionsCreateSingleVM request)
        {
			var checkVariant = await _dbcontext.ProductDetails.FirstOrDefaultAsync(pd => pd.ID == request.IDProductDetails);
			if (checkVariant == null)
			{

				return false; 
			}

			var checkColor = await _dbcontext.Colors.FirstOrDefaultAsync(c => c.ID == request.IDColor);
			var checkSize = await _dbcontext.Sizes.FirstOrDefaultAsync(s => s.ID == request.IDSize);

			if (checkColor == null && !string.IsNullOrEmpty(request.ColorName))
			{
				request.IDColor = await EnsureColor(request.ColorName, request.CreateBy);
			}
			if (checkSize == null && !string.IsNullOrEmpty(request.SizesName))
			{
				request.IDSize = await EnsureSize(request.SizesName, request.CreateBy);
			}

			var existingOption = await _dbcontext.Options.FirstOrDefaultAsync(o =>
				o.IDProductDetails == request.IDProductDetails &&
				o.IDColor == request.IDColor &&
				o.IDSize == request.IDSize);

			if (existingOption != null)
			{
				return false;
			}

			string uploadedImageUrl = null;
			if (request.ImageURL != null)
			{
				uploadedImageUrl = await UploadImageAsync(request.ImageURL);
			}

			var newOption = new Options
			{
				ID = Guid.NewGuid(),
				IDProductDetails = request.IDProductDetails,
				IDColor = request.IDColor,
				IDSize = request.IDSize,
				StockQuantity = request.StockQuantity,
				RetailPrice = request.RetailPrice,
				Discount = request.Discount,
				IsActive = request.IsActive,
				ImageURL =uploadedImageUrl,
				Status = 1,
				CreateBy = request.CreateBy,
			};

			_dbcontext.Options.Add(newOption);
			await _dbcontext.SaveChangesAsync();

			return true;
		}
        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null)
            {
                throw new ArgumentNullException(nameof(imageFile));
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                Folder = "BeyoungSportWear/ImageProduct/Options"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }
            else
            {
                throw new Exception("Failed to upload image to Cloudinary");
            }
        }
        public async Task<List<OptionsVM>> GetAllActiveAsync()
        {
            var objList = await _dbcontext.Options
                           .AsNoTracking()
                           .Where(b => b.Status != 0 && b.IsActive != false & b.StockQuantity != 0)
                           .ProjectTo<OptionsVM>(_mapper.ConfigurationProvider)
                           .ToListAsync();

            return objList ?? new List<OptionsVM>();
        }
        public async Task<List<OptionsVM>> GetAllAsync()
        {
            var objList = await _dbcontext.Options
                             .AsNoTracking()
                             .ProjectTo<OptionsVM>(_mapper.ConfigurationProvider)
                             .ToListAsync();

            return objList ?? new List<OptionsVM>();
        }
        public async Task<OptionsVM> GetByIDAsync(Guid ID)
        {
            var obj = await _dbcontext.Options
                           .Where(o => o.ID == ID)
                           .ProjectTo<OptionsVM>(_mapper.ConfigurationProvider)
                           .FirstOrDefaultAsync();

            return obj;
        }
        public async Task<bool> RemoveAsync(Guid ID, string IDUserdelete)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Options.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null)
                    {
                        obj.Status = 0;
                        obj.DeleteDate = DateTime.Now;
                        obj.DeleteBy = IDUserdelete;

                        _dbcontext.Options.Attach(obj);
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
        public async Task<bool> UpdateAsync(Guid ID, OptionsUpdateVM request)
        {
            var checkColor = await _dbcontext.Colors.FirstOrDefaultAsync(c => c.ID == request.IDColor);
            var checkSizes = await _dbcontext.Sizes.FirstOrDefaultAsync(c => c.ID == request.IDSize);


            if (checkSizes == null && !string.IsNullOrEmpty(request.SizesName))
            {
                request.IDSize = await EnsureSize(request.SizesName, request.ModifiedBy);
            }

            if (checkColor == null && !string.IsNullOrEmpty(request.ColorName))
            {
                request.IDColor = await EnsureColor(request.ColorName, request.ModifiedBy);
            }

            checkColor = await _dbcontext.Colors.FirstOrDefaultAsync(c => c.ID == request.IDColor);
            checkSizes = await _dbcontext.Sizes.FirstOrDefaultAsync(c => c.ID == request.IDSize);

            var option = await _dbcontext.Options.FindAsync(ID);

            if (option == null)
            {
                return false;
            }
            if (request.ImageURL != null)
            {
                try
                {
                    var imageUrl = await UploadImageAsync(request.ImageURL);
                    option.ImageURL = imageUrl;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to upload image", ex);
                }
            }

            option.IDColor = checkColor.ID;
            option.IDSize = checkSizes.ID;
            option.StockQuantity = request.StockQuantity;
            option.RetailPrice = request.RetailPrice;
            option.Status = 1;
            option.IsActive = request.IsActive;
            option.ModifiedBy = request.ModifiedBy;
            option.ModifiedDate = DateTime.Now;

            _dbcontext.Options.Update(option);
            await _dbcontext.SaveChangesAsync();

            return true;
        }
        public async Task<string> UploadImageToCloudinary(IFormFile file)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
            };

            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.AbsoluteUri;
            }
            catch (Exception)
            {
                return "Không thể upload hình ảnh";
            }
        }  
        public async Task<bool> UpdateIsActiveAsync(Guid IDOptions, bool isActive)
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                try
                {
                    var option = await _dbcontext.Options
                        .Include(o => o.ProductDetails)  
                        .FirstOrDefaultAsync(o => o.ID == IDOptions);

                    if (option == null)
                    {
                        return false;
                    }

                    var productDetailIsActive = option.ProductDetails.IsActive;
                    if (!productDetailIsActive)
                    {
                        return false;
                    }

                    option.IsActive = isActive;
                    _dbcontext.Options.Update(option);
                    await _dbcontext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error updating option IsActive", ex);
                }
            }
        }
        public async Task<Result> DecreaseQuantityAsync(Guid IDOptions, int quantityToDecrease)
        {
            if (quantityToDecrease <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Số lượng giảm phải lớn hơn 0."
                };
            }
            var option = await _dbcontext.Options.FindAsync(IDOptions);
            if (option == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Không tìm thấy sản phẩm với ID tương ứng."
                };
            }

            if (option.StockQuantity < quantityToDecrease)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Số lượng trong kho không đủ để giảm."
                };
            }

            option.StockQuantity -= quantityToDecrease;
            _dbcontext.Options.Update(option);
            await _dbcontext.SaveChangesAsync();

            return new Result
            {
                Success = true,
                ErrorMessage = "Giảm số lượng sản phẩm thành công."
            };
        }
        public async Task<Result> IncreaseQuantityAsync(Guid IDOptions, int quantityToIncrease)
        {
            if (quantityToIncrease <= 0)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Số lượng tăng phải lớn hơn 0."
                };
            }

            var option = await _dbcontext.Options.FindAsync(IDOptions);
            if (option == null)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = "Không tìm thấy sản phẩm với ID tương ứng."
                };
            }

            option.StockQuantity += quantityToIncrease;
            _dbcontext.Options.Update(option);
            await _dbcontext.SaveChangesAsync();

            return new Result
            {
                Success = true,
                ErrorMessage = "Tăng số lượng sản phẩm thành công."
            };
        }
        public async Task<OptionsVM> FindIDOptionsAsync(Guid IDProductDetails, string size, string color)
        {
            var option = await _dbcontext.Options
                 .Include(o => o.Colors)
                 .Include(o => o.Sizes)
                 .Where(o =>
                     o.IDProductDetails == IDProductDetails &&
                     (string.IsNullOrEmpty(size) || o.Sizes.Name == size) &&
                     (string.IsNullOrEmpty(color) || o.Colors.Name == color))
                 .ProjectTo<OptionsVM>(_mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync();

            return option;
        }
        public async Task<List<OptionsVM>> FindIDOptionsBySize(Guid IDProductDetails, string size)
        {
            var options = await _dbcontext.Options
                .Include(o => o.Sizes) 
                .Where(b =>
                    b.IDProductDetails == IDProductDetails &&
                    b.Sizes.Name == size && b.Status != 0 && b.IsActive != false & b.StockQuantity != 0)
                .ProjectTo<OptionsVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return _mapper.Map<List<OptionsVM>>(options);
        }
        public async Task<List<OptionsVM>> FindIDOptionsByColor(Guid IDProductDetails, string color)
        {
            var option = await _dbcontext.Options
                .Include(o => o.Colors)
                .Where(b =>
                    b.IDProductDetails == IDProductDetails &&
                    b.Colors.Name == color && b.Status != 0 && b.IsActive != false & b.StockQuantity != 0)
                .ProjectTo<OptionsVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return _mapper.Map<List<OptionsVM>>(option);
        }
        public async Task<List<OptionsVM>> GetByNameAsync(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return new List<OptionsVM>();
            }

            var options = await _dbcontext.Options
                .Where(o => o.ProductDetails.Products.Name.Contains(Name)) 
                .ProjectTo<OptionsVM>(_mapper.ConfigurationProvider) 
                .ToListAsync();

            return options;
        }
    }
}

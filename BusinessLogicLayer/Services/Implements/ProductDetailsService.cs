using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.CartOptions;
using BusinessLogicLayer.Viewmodels.Image;
using BusinessLogicLayer.Viewmodels.Options;
using BusinessLogicLayer.Viewmodels.ProductDetails;
using CloudinaryDotNet;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using static DataAccessLayer.Entity.Base.EnumBase;

namespace BusinessLogicLayer.Services.Implements
{
	public class ProductDetailsService : IProductDetailsService
	{

        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IManufacturerService _IManufacturerService;
        private readonly IBrandService _IBrandService;
        private readonly IMaterialService _IMaterialService;
        public ProductDetailsService(ApplicationDBContext ApplicationDBContext, IMapper mapper, 
            Cloudinary Cloudinary, IManufacturerService iManufacturerService,
            IMaterialService IMaterialService, IBrandService IBrandService)
        {
            _cloudinary = Cloudinary;
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
            _IManufacturerService = iManufacturerService;
            _IMaterialService = IMaterialService;
            _IBrandService = IBrandService;
        }
        public async Task<Guid> EnsureCategory(string CategoryName, string CreateBy)
        {
            var category = await _dbcontext.Category
                .FirstOrDefaultAsync(s => s.Name.ToLower() == CategoryName.ToLower());

			if (category == null)
			{
				category = new Category
				{
					ID = Guid.NewGuid(),
					Name = CategoryName,
					Status = 1,
					CreateBy = CreateBy,
					CreateDate = DateTime.Now
				};
				await _dbcontext.Category.AddAsync(category);
				await _dbcontext.SaveChangesAsync();
			}

			return category.ID;
		}
		public async Task<Guid> EnsureBrand(string BrandName, string CreateBy)
		{
			var brand = await _dbcontext.Brand
				.FirstOrDefaultAsync(c => c.Name.ToLower() == BrandName.ToLower());

			if (brand == null)
			{
				brand = new Brand
				{
					ID = Guid.NewGuid(),
					Name = BrandName,
					Description = "",
					Address = "",
					Phone = "",
					Gmail = "",
					Website = "",
					Status = 1,
					CreateBy = CreateBy,
					CreateDate = DateTime.Now
				};
				await _dbcontext.Brand.AddAsync(brand);
				await _dbcontext.SaveChangesAsync();
			}

			return brand.ID;
		}
		public async Task<Guid> EnsureManufacturers(string ManufacturersName, string CreateBy)
		{
			var manufacturers = await _dbcontext.Manufacturer
				.FirstOrDefaultAsync(c => c.Name.ToLower() == ManufacturersName.ToLower());

			if (manufacturers == null)
			{
				manufacturers = new Manufacturer
				{
					ID = Guid.NewGuid(),
					Name = ManufacturersName,
					Description = "",
					Address = "",
					Phone = "",
					Gmail = "",
					Website = "",
					Status = 1,
					CreateDate = DateTime.Now,
					CreateBy = CreateBy,
				};
				await _dbcontext.Manufacturer.AddAsync(manufacturers);
				await _dbcontext.SaveChangesAsync();
			}

			return manufacturers.ID;
		}
		public async Task<Guid> EnsureMaterial(string materialName, string CreateBy)
		{
			var material = await _dbcontext.Material
				.FirstOrDefaultAsync(c => c.Name.ToLower() == materialName.ToLower());

			if (material == null)
			{
				material = new Material
				{
					ID = Guid.NewGuid(),
					Name = materialName,
					Status = 1,
					CreateDate = DateTime.Now,
					CreateBy = CreateBy,
				};
				await _dbcontext.Material.AddAsync(material);
				await _dbcontext.SaveChangesAsync();
			}

			return material.ID;
		}
		public async Task<Guid> EnsureProduct(string productTypeName, string CreateBy)
		{
			var productType = await _dbcontext.Product
				.FirstOrDefaultAsync(c => c.Name.ToLower() == productTypeName.ToLower());

			if (productType == null)
			{
				productType = new Product
				{
					ID = Guid.NewGuid(),
					Name = productTypeName,
					Status = 1,
					CreateDate = DateTime.Now,
					CreateBy = CreateBy,
				};
				await _dbcontext.Product.AddAsync(productType);
				await _dbcontext.SaveChangesAsync();
			}

			return productType.ID;
		}
		public async Task<Guid> EnsureSize(string sizeName, string CreateBy)
		{
			var size = await _dbcontext.Sizes
				.FirstOrDefaultAsync(s => s.Name.ToLower() == sizeName.ToLower());

			if (size == null)
			{
				size = new Sizes
				{
					ID = Guid.NewGuid(),
					Name = sizeName,
					CreateDate = DateTime.Now,
					CreateBy = CreateBy,
					Status = 1
				};
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
				color = new Colors
				{
					ID = Guid.NewGuid(),
					Name = colorName,
					CreateDate = DateTime.Now,
					CreateBy = CreateBy,
					Status = 1
				};
				await _dbcontext.Colors.AddAsync(color);
				await _dbcontext.SaveChangesAsync();
			}

            return color.ID;
        }
        //private string GenerateKeyCode(string productName)
        //{
        //    string normalizedProductName = RemoveDiacritics(productName);
        //    string keyPart = GetInitials(normalizedProductName);
        //    var random = new Random();
        //    string keyCode = $"{keyPart}-{random.Next(1000, 9999)}";

        //    return keyCode.ToUpper();
        //}
        //private string RemoveDiacritics(string text)
        //{
        //    var normalizedString = text.Normalize(NormalizationForm.FormD);
        //    var stringBuilder = new StringBuilder();

        //    foreach (var c in normalizedString)
        //    {
        //        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
        //        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
        //        {
        //            stringBuilder.Append(c);
        //        }
        //    }

        //    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        //}
        //private string GetInitials(string text)
        //{
        //    var words = text.Split(' ');
        //    var initials = new StringBuilder();

        //    foreach (var word in words)
        //    {
        //        if (!string.IsNullOrEmpty(word) && char.IsLetterOrDigit(word[0]))
        //        {
        //            initials.Append(word[0]);
        //        }
        //    }

        //    return initials.ToString().ToUpper();
        //}
        public async Task<bool> CreateAsync(ProductDetailsCreateVM request)
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                try
                {
                    var checkBrand = await _dbcontext.Brand.FirstOrDefaultAsync(c => c.ID == request.IDBrand);
                    var checkMaterial = await _dbcontext.Material.FirstOrDefaultAsync(c => c.ID == request.IDMaterial);
                    var checkManufacturer = await _dbcontext.Manufacturer.FirstOrDefaultAsync(c => c.ID == request.IDManufacturers);
                    var checkProduct = await _dbcontext.Product.FirstOrDefaultAsync(c => c.ID == request.IDProduct);
                    var checkCategory = await _dbcontext.Category.FirstOrDefaultAsync(c => c.ID == request.IDCategory);

					if (checkBrand == null && !string.IsNullOrEmpty(request.BrandName))
					{
						request.IDBrand = await EnsureBrand(request.BrandName, request.CreateBy);
					}
					if (checkCategory == null && !string.IsNullOrEmpty(request.CategoryName))
					{
						request.IDCategory = await EnsureCategory(request.CategoryName, request.CreateBy);
					}

					if (checkProduct == null && !string.IsNullOrEmpty(request.ProductName))
					{
						request.IDProduct = await EnsureProduct(request.ProductName, request.CreateBy);
					}

					if (checkMaterial == null && !string.IsNullOrEmpty(request.MaterialName))
					{
						request.IDMaterial = await EnsureMaterial(request.MaterialName, request.CreateBy);
					}

					if (checkManufacturer == null && !string.IsNullOrEmpty(request.ManufacturersName))
					{
						request.IDManufacturers = await EnsureManufacturers(request.ManufacturersName, request.CreateBy);
					}

					checkBrand = await _dbcontext.Brand.FirstOrDefaultAsync(c => c.ID == request.IDBrand);
					checkMaterial = await _dbcontext.Material.FirstOrDefaultAsync(c => c.ID == request.IDMaterial);
					checkManufacturer = await _dbcontext.Manufacturer.FirstOrDefaultAsync(c => c.ID == request.IDManufacturers);
					checkProduct = await _dbcontext.Product.FirstOrDefaultAsync(c => c.ID == request.IDProduct);
					checkCategory = await _dbcontext.Category.FirstOrDefaultAsync(c => c.ID == request.IDCategory);

					if (checkBrand == null || checkMaterial == null || checkManufacturer == null || checkProduct == null || checkCategory == null)
					{
						throw new Exception("Brand, Material, Manufacturer, Category or Product not found.");
					}


					var productDetails = new ProductDetails
					{
						ID = request.ID,
						IDMaterial = checkMaterial.ID,
						IDBrand = checkBrand.ID,
						IDManufacturers = checkManufacturer.ID,
						IDProduct = checkProduct.ID,
						IDCategory = checkCategory.ID,
						Origin = request.Origin,
						Style = request.Style,
						KeyCode = request.KeyCode,
						Description = request.Description,
						IsActive = request.IsActive,
						CreateBy = request.CreateBy,
						CreateDate = DateTime.Now,
						Status = 1,
					};
					await _dbcontext.ProductDetails.AddAsync(productDetails);
					if (request.OptionsCreateVM == null || !request.OptionsCreateVM.Any())
					{
						throw new Exception("OptionsCreateVM is null or empty");
					}

					foreach (var option in request.OptionsCreateVM)
					{
						var checkColor = await _dbcontext.Colors.FirstOrDefaultAsync(c => c.ID == option.IDColor);
						var checkSizes = await _dbcontext.Sizes.FirstOrDefaultAsync(c => c.ID == option.IDSize);

						if (checkSizes == null && !string.IsNullOrEmpty(option.SizesName))
						{
							option.IDSize = await EnsureSize(option.SizesName, request.CreateBy);
						}

						if (checkColor == null && !string.IsNullOrEmpty(option.ColorName))
						{
							option.IDColor = await EnsureColor(option.ColorName, request.CreateBy);
						}

						checkColor = await _dbcontext.Colors.FirstOrDefaultAsync(c => c.ID == option.IDColor);
						checkSizes = await _dbcontext.Sizes.FirstOrDefaultAsync(c => c.ID == option.IDSize);

						if (checkColor == null || checkSizes == null)
						{
							throw new Exception("Color or Size not found.");
						}

                        var options = new Options
                        {
                            ID = Guid.NewGuid(),
                            IDProductDetails = productDetails.ID,
                            IDColor = checkColor.ID,
                            IDSize = checkSizes.ID,
                            StockQuantity = option.StockQuantity,
                            RetailPrice = option.RetailPrice,
                            ImageURL = option.ImageURL,
                            CreateBy = request.CreateBy,
                            CreateDate = DateTime.Now,
                            Status = 1
                        };
                        _dbcontext.Options.Add(options);
                    }
                    await _dbcontext.SaveChangesAsync();


					await _dbcontext.SaveChangesAsync();
					await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error creating product details", ex);
                }
            }
        }
        public async Task<List<ProductDetailsVM>> GetAllActiveAsync(int pageIndex, int pageSize)
        {
            var activeVariants = await _dbcontext.ProductDetails
             .AsNoTracking()
             .OrderBy(p => p.Status == 0 ? 1 : 0)
             .ThenByDescending(p => p.CreateDate)
             .Skip(pageIndex * pageSize)
             .Take(pageSize)
             .Select(v => new
             {
                 v.ID,
                 ProductName = v.Products.Name,
                 CategoryName = v.Category.Name,
                 TotalQuantity = v.Options.Sum(opt => opt.StockQuantity),
                 KeyCode = v.KeyCode,
                 Description = v.Description,
                 Origin = v.Origin,
                 Status = v.Status,
                 IsActive = v.IsActive,
                 Style = v.Style,
                 SmallestPrice = v.Options.Any() ? v.Options.Min(opt => opt.RetailPrice) : 0,
                 BiggestPrice = v.Options.Any() ? v.Options.Max(opt => opt.RetailPrice) : 0,
                 ImagePaths = v.Images.Where(m => m.Status == 1).Select(m => m.Path).ToList(),
                 CreateDate = v.CreateDate
             })
             .AsSplitQuery()
             .ToListAsync();

            var variantsVMList = activeVariants.Select(v => new ProductDetailsVM
            {
                ID = v.ID,
                ProductName = v.ProductName,
                CategoryName = v.CategoryName,
                TotalQuantity = v.TotalQuantity,
                KeyCode = v.KeyCode,
                IsActive = v.IsActive,
                Description = v.Description,
                Origin = v.Origin,
                Status = v.Status,
                Style = v.Style,
                SmallestPrice = v.SmallestPrice,
                BiggestPrice = v.BiggestPrice,
                ImagePaths = v.ImagePaths,
                CreateDate = v.CreateDate
            }).Where(b => b.Status != 0 && b.IsActive != false).ToList();

			return variantsVMList;

        }
        public async Task<List<ProductDetailsVM>> GetAllAsync(int pageIndex, int pageSize)
        {
            var activeVariants = await _dbcontext.ProductDetails
                .AsNoTracking()
                .OrderBy(p => p.Status == 0 ? 1 : 0)
                .ThenByDescending(p => p.CreateDate)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(v => new
                {
                    v.ID,
                    ProductName = v.Products.Name,
                    CategoryName = v.Category.Name,
                    TotalQuantity = v.Options.Sum(opt => opt.StockQuantity),
                    KeyCode = v.KeyCode,
                    Description = v.Description,
                    IsActive = v.IsActive,
                    Origin = v.Origin,
                    Status = v.Status,
                    Style = v.Style,
                    SmallestPrice = v.Options.Any() ? v.Options.Min(opt => opt.RetailPrice) : 0,
                    BiggestPrice = v.Options.Any() ? v.Options.Max(opt => opt.RetailPrice) : 0,
                    ImagePaths = v.Images.Where(m => m.Status == 1).Select(m => m.Path).ToList(),
                    CreateDate = v.CreateDate 
                })
                .AsSplitQuery() 
                .ToListAsync();

            var variantsVMList = activeVariants.Select(v => new ProductDetailsVM
            {
                ID = v.ID,
                ProductName = v.ProductName,
                CategoryName = v.CategoryName,
                TotalQuantity = v.TotalQuantity,
                KeyCode = v.KeyCode,
                Description = v.Description,
                Origin = v.Origin,
                IsActive = v.IsActive,
                Status = v.Status,
                Style = v.Style,
                SmallestPrice = v.SmallestPrice,
                BiggestPrice = v.BiggestPrice,
                ImagePaths = v.ImagePaths,
                CreateDate = v.CreateDate 
            }).ToList();

            return variantsVMList;
        }
        public async Task<ProductDetailsVM> GetByIDAsync(Guid ID)
        {
            var productDetails = await _dbcontext.ProductDetails
                   .Include(pd => pd.Products)
                   .Include(pd => pd.Category)
                   .Include(pd => pd.Manufacturers)
                   .Include(pd => pd.Material)
                   .Include(pd => pd.Brand)
                   .Include(pd => pd.Options)
                   .ThenInclude(opt => opt.Colors)
                   .Include(pd => pd.Options)
                   .ThenInclude(opt => opt.Sizes)
                   .Include(pd => pd.Images)
                   .Where(pd => pd.ID == ID)
                   .Select(pd => new ProductDetailsVM
                   {
                       ID = pd.ID,
                       CreateBy = pd.CreateBy,
                       CreateDate = pd.CreateDate,
                       IDProduct = pd.Products.ID,
                       SmallestPrice = pd.Options.Any() ? pd.Options.Min(opt => opt.RetailPrice) : 0,
                       BiggestPrice = pd.Options.Any() ? pd.Options.Max(opt => opt.RetailPrice) : 0,
                       ProductName = pd.Products.Name,
                       IDCategory = pd.Category.ID,
                       CategoryName = pd.Category.Name,
                       IDManufacturers = pd.Manufacturers.ID,
                       ManufacturersName = pd.Manufacturers.Name,
                       IDMaterial = pd.Material.ID,
                       KeyCode = pd.KeyCode,
                       TotalQuantity = pd.Options.Sum(opt => opt.StockQuantity),
                       MaterialName = pd.Material.Name,
                       IDBrand = pd.Brand.ID,
                       BrandName = pd.Brand.Name,
                       Description = pd.Description,
                       IsActive = pd.IsActive,
                       Style = pd.Style,
                       Origin = pd.Origin,
                       ImagePaths = pd.Images.Select(img => img.Path).ToList(),
                       Status = pd.Status,
                       Options = pd.Options.Select(o => new OptionsVM
                       {
                           ID = o.ID,
                           IDColor = o.Colors.ID,
                           ColorName = o.Colors.Name,
                           IDSize = o.Sizes.ID,
                           SizesName = o.Sizes.Name,
                           StockQuantity = o.StockQuantity,
                           IsActive = o.IsActive,
                           RetailPrice = o.RetailPrice,
                           ImageURL = o.ImageURL,
                           CreateDate = o.CreateDate,
                           CreateBy = o.CreateBy,
                           Status = o.Status,
                       }).ToList()
                   }).FirstOrDefaultAsync();

			return productDetails;
		}
		public async Task<List<OptionsVM>> GetOptionProductDetailsByIDAsync(Guid IDProductDetails)
		{
			var optionsList = await _dbcontext.Options
								  .AsNoTracking()
								  .Where(opt => opt.IDProductDetails == IDProductDetails && opt.IsActive != false && opt.Status != 0)
								  .Select(opt => new OptionsVM
								  {
									  ID = opt.ID,
									  IDProductDetails = opt.IDProductDetails,
									  SizesName = opt.Sizes.Name,
									  IDSize = opt.Sizes.ID,
									  ColorName = opt.Colors.Name,
									  IDColor = opt.Colors.ID,
									  StockQuantity = opt.StockQuantity,
									  CreateBy = opt.CreateBy,
									  ImageURL = opt.ImageURL,
									  CreateDate = opt.CreateDate,
									  RetailPrice = opt.RetailPrice,
									  Discount = opt.Discount,
									  Status = opt.Status
								  })
								  .ToListAsync();

			return optionsList;
		}
		public async Task<bool> RemoveAsync(Guid ID, string IDUserDelete)
		{
			using (var transaction = _dbcontext.Database.BeginTransaction())
			{
				try
				{
					var obj = await _dbcontext.ProductDetails.FirstOrDefaultAsync(c => c.ID == ID);

					if (obj != null)
					{
						obj.Status = 0;
						obj.DeleteDate = DateTime.Now;
						obj.DeleteBy = IDUserDelete;

						_dbcontext.ProductDetails.Attach(obj);
						var relatedOptions = await _dbcontext.Options.Where(c => c.IDProductDetails == ID).ToListAsync();
						foreach (var options in relatedOptions)
						{
							options.Status = 0;
						}


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
        public async Task<bool> UpdateAsync(Guid ID, ProductDetailsUpdateVM request)
        {
            using (var transaction = await _dbcontext.Database.BeginTransactionAsync())
            {
                try
                {
                    var productDetail = await _dbcontext.ProductDetails
                        .Include(pd => pd.Options)
                        .ThenInclude(o => o.Colors)
                        .Include(pd => pd.Options)
                        .ThenInclude(o => o.Sizes)
                        .FirstOrDefaultAsync(pd => pd.ID == ID);

                    if (productDetail == null)
                    {
                        return false;
                    }

                    var brandId = await EnsureBrand(request.BrandName, request.ModifiedBy);
                    var categoryId = await EnsureCategory(request.CategoryName, request.ModifiedBy);
                    var productId = await EnsureProduct(request.ProductName, request.ModifiedBy);
                    var materialId = await EnsureMaterial(request.MaterialName, request.ModifiedBy);
                    var manufacturerId = await EnsureManufacturers(request.ManufacturersName, request.ModifiedBy);

                    productDetail.Description = request.Description;
                    productDetail.Style = request.Style;
                    productDetail.Origin = request.Origin;
                    productDetail.IDBrand = brandId;
                    productDetail.IDCategory = categoryId;
                    productDetail.IDProduct = productId;
                    productDetail.IDMaterial = materialId;
                    productDetail.IDManufacturers = manufacturerId;
                    productDetail.Status = 1;
                    productDetail.ModifiedBy = request.ModifiedBy;
                    productDetail.ModifiedDate = DateTime.Now;

                    _dbcontext.ProductDetails.Update(productDetail);

                    var existingColors = await _dbcontext.Colors.ToDictionaryAsync(c => c.ID);
                    var existingSizes = await _dbcontext.Sizes.ToDictionaryAsync(s => s.ID);

                    foreach (var optionVM in request.NewOptions)
                    {
                        if (!existingSizes.ContainsKey(optionVM.IDSize) && !string.IsNullOrEmpty(optionVM.SizesName))
                        {
                            optionVM.IDSize = await EnsureSize(optionVM.SizesName, request.ModifiedBy);
                        }

                        if (!existingColors.ContainsKey(optionVM.IDColor) && !string.IsNullOrEmpty(optionVM.ColorName))
                        {
                            optionVM.IDColor = await EnsureColor(optionVM.ColorName, request.ModifiedBy);
                        }

                        if (!existingColors.ContainsKey(optionVM.IDColor) || !existingSizes.ContainsKey(optionVM.IDSize))
                        {
                            throw new Exception("Color or Size not found.");
                        }

                        var existingOption = productDetail.Options.FirstOrDefault(o =>
                            o.IDProductDetails == productDetail.ID &&
                            o.IDColor == optionVM.IDColor &&
                            o.IDSize == optionVM.IDSize);

                        if (existingOption != null)
                        {
                            existingOption.RetailPrice = optionVM.RetailPrice;
                            existingOption.StockQuantity = optionVM.StockQuantity;
                            existingOption.ImageURL = optionVM.ImageURL;
                        }
                        else
                        {
                            var newOption = new Options
                            {
                                ID = Guid.NewGuid(),
                                IDProductDetails = productDetail.ID,
                                IDSize = optionVM.IDSize,
                                IDColor = optionVM.IDColor,
                                RetailPrice = optionVM.RetailPrice,
                                StockQuantity = optionVM.StockQuantity,
                                IsActive = optionVM.IsActive,
                                ImageURL = optionVM.ImageURL,
                                CreateBy = request.ModifiedBy,
                                CreateDate = DateTime.Now,
                                Status = 1
                            };

                            _dbcontext.Options.Add(newOption);
                        }
                    }

                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error updating product details", ex);
                }
            }
        }
        public IQueryable<ProductDetailsVM> Search(List<SearchCondition> conditions)
		{
			var query = _dbcontext.ProductDetails
				.Include(p => p.Products)
				.Include(p => p.Brand)
				.Include(p => p.Material)
				.Include(p => p.Category)
				.Include(p => p.Manufacturers)
				.Include(p => p.Images.Where(i => i.Status == 1))
				.AsQueryable();

			foreach (var condition in conditions)
			{
				switch (condition.Criteria)
				{
					case SearchCriteria.Product:
						query = query.Where(p => p.Products.Name.Contains(condition.Value));
						break;

					case SearchCriteria.Material:
						query = query.Join(_dbcontext.Set<Material>(),
										   p => p.IDMaterial,
										   m => m.ID,
										   (p, m) => new { p, m })
									 .Where(joined => joined.m.Name.Contains(condition.Value))
									 .Select(joined => joined.p);
						break;

					case SearchCriteria.Brand:
						query = query.Join(_dbcontext.Set<Brand>(),
										   p => p.IDBrand,
										   b => b.ID,
										   (p, b) => new { p, b })
									 .Where(joined => joined.b.Name.Contains(condition.Value))
									 .Select(joined => joined.p);
						break;

					case SearchCriteria.Category:
						query = query.Join(_dbcontext.Set<Category>(),
										   p => p.IDCategory,
										   c => c.ID,
										   (p, c) => new { p, c })
									 .Where(joined => joined.c.Name.Contains(condition.Value))
									 .Select(joined => joined.p);
						break;

					case SearchCriteria.Manufacturer:
						query = query.Join(_dbcontext.Set<Manufacturer>(),
										   p => p.IDManufacturers,
										   mf => mf.ID,
										   (p, mf) => new { p, mf })
									 .Where(joined => joined.mf.Name.Contains(condition.Value))
									 .Select(joined => joined.p);
						break;

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

            var result = query.Select(p => new ProductDetailsVM
            {
                ID = p.ID,
                IDProduct = p.Products.ID,
                ProductName = p.Products.Name,
                IDBrand = p.Brand.ID,
                BrandName = p.Brand.Name,
                IDCategory = p.Category.ID,
                CategoryName = p.Category.Name,
                TotalQuantity = p.Options.Sum(opt => opt.StockQuantity),
                IDManufacturers = p.Manufacturers.ID,
                ManufacturersName = p.Manufacturers.Name,
                IDMaterial = p.Material.ID,
                MaterialName = p.Material.Name,
                KeyCode = p.KeyCode,
                Description = p.Description,
                Origin = p.Origin,
                Status = p.Status,
                Style = p.Style,
                SmallestPrice = p.Options.Any() ? p.Options.Min(opt => opt.RetailPrice) : 0,
                BiggestPrice = p.Options.Any() ? p.Options.Max(opt => opt.RetailPrice) : 0,
                ImagePaths = p.Images.Select(i => i.Path).ToList()
            });

			return result;
		}
		public async Task<bool> UpdateIsActiveAsync(Guid IDProductDetails, bool isActive)
		{

			try
			{
				var productDetail = await _dbcontext.ProductDetails.FirstOrDefaultAsync(pd => pd.ID == IDProductDetails);
				if (productDetail == null)
				{
					return false;
				}

				productDetail.IsActive = isActive;

                _dbcontext.ProductDetails.Update(productDetail);
                await _dbcontext.SaveChangesAsync();

				return true;
			}
			catch (Exception ex)
			{
				throw new Exception("Error updating IsActive", ex);
			}
		}
		public async Task<ProductDetailUser> GetProductDetailInfo(Guid IDProductDetails, string size, string color)
		{
			var productDetails = await _dbcontext.ProductDetails
							.Where(pd => pd.ID == IDProductDetails)
							.Include(pd => pd.Products)
							.Include(pd => pd.Options)
								.ThenInclude(o => o.Colors)
							.Include(pd => pd.Options)
								.ThenInclude(o => o.Sizes)
							.FirstOrDefaultAsync();

			if (productDetails == null)
			{
				throw new KeyNotFoundException("Không tìm thấy sản phẩm.");
			}

			var option = productDetails.Options
				.FirstOrDefault(o => o.Sizes.Name == size && o.Colors.Name == color);

			if (option == null)
			{
				throw new KeyNotFoundException("Không tìm thấy tuỳ chọn sản phẩm với size và color đã chọn.");
			}

			var sizes = productDetails.Options.Select(o => o.Sizes.Name).Distinct().ToList();
			var colors = productDetails.Options.Select(o => o.Colors.Name).Distinct().ToList();
			var image = option?.ImageURL ?? null;
			var quantity = option?.StockQuantity ?? 0;
			var price = option?.RetailPrice ?? 0;
			var description = productDetails.Description;

			return new ProductDetailUser
			{
				ID = productDetails.ID,
				IDOptions = option.ID.ToString(),
				Name = productDetails.Products.Name,
				Price = price,
				Description = description,
				Size = sizes,
				Color = colors,
				UrlImg = image,
				Quantity = quantity,
			};
		}
		public async Task<ProductDetailsVM> GetByIDAsyncVer_1(Guid ID)
		{
			var productDetails = await _dbcontext.ProductDetails
							   .Include(pd => pd.Products)
							   .Include(pd => pd.Category)
							   .Include(pd => pd.Manufacturers)
							   .Include(pd => pd.Material)
							   .Include(pd => pd.Brand)
							   .Include(pd => pd.Options)
							   .ThenInclude(opt => opt.Colors)
							   .Include(pd => pd.Options)
							   .ThenInclude(opt => opt.Sizes)
							   .Include(pd => pd.Images)
							   .Where(pd => pd.ID == ID)
							   .Select(pd => new ProductDetailsVM
							   {
								   ID = pd.ID,
								   CreateBy = pd.CreateBy,
								   CreateDate = pd.CreateDate,
								   IDProduct = pd.Products.ID,
								   SmallestPrice = pd.Options.Any() ? pd.Options.Min(opt => opt.RetailPrice) : 0,
								   BiggestPrice = pd.Options.Any() ? pd.Options.Max(opt => opt.RetailPrice) : 0,
								   ProductName = pd.Products.Name,
								   IDCategory = pd.Category.ID,
								   CategoryName = pd.Category.Name,
								   IDManufacturers = pd.Manufacturers.ID,
								   ManufacturersName = pd.Manufacturers.Name,
								   IDMaterial = pd.Material.ID,
								   KeyCode = pd.KeyCode,
								   TotalQuantity = pd.Options.Sum(opt => opt.StockQuantity),
								   MaterialName = pd.Material.Name,
								   IDBrand = pd.Brand.ID,
								   BrandName = pd.Brand.Name,
								   Description = pd.Description,
								   IsActive = pd.IsActive,
								   Style = pd.Style,
								   Origin = pd.Origin,
								   ImagePaths = pd.Images.Select(img => img.Path).ToList(),
								   Status = pd.Status,
								   Options = pd.Options.Select(o => new OptionsVM
								   {
									   ID = o.ID,
									   IDColor = o.Colors.ID,
									   ColorName = o.Colors.Name,
									   IDSize = o.Sizes.ID,
									   SizesName = o.Sizes.Name,
									   StockQuantity = o.StockQuantity,
									   IsActive = o.IsActive,
									   RetailPrice = o.RetailPrice,
									   ImageURL = o.ImageURL,
									   CreateDate = o.CreateDate,
									   CreateBy = o.CreateBy,
									   Status = o.Status,
								   }).ToList()
							   }).FirstOrDefaultAsync();

			return productDetails;
		}

        public async Task<List<ProductDetailsVM>> GetProductsByPriceRangeAsync(/*Guid IDProduct,*/ decimal minPrice, decimal maxPrice)
        {
            var products = await _dbcontext.ProductDetails
                                              .Include(p => p.Products)
                                              .Include(p => p.Options)
                                              .Include(p => p.Images)
                                              .Include(p => p.Category)
                                              .Where(p => /*p.IDProduct == IDProduct &&*/ 
											  p.Options.Any(opt => opt.RetailPrice >= minPrice && opt.RetailPrice <= maxPrice))
                                              .ToListAsync();

            return products.Select(p => new ProductDetailsVM
            {
                ID = p.ID,
                ProductName = p.Products != null ? p.Products.Name : string.Empty,
                SmallestPrice = p.Options != null && p.Options.Any() ? p.Options.Min(opt => opt.RetailPrice) : 0,
                BiggestPrice = p.Options != null && p.Options.Any() ? p.Options.Max(opt => opt.RetailPrice) : 0,
                ImagePaths = p.Images != null ? p.Images.Where(m => m.Status == 1).Select(m => m.Path).ToList() : new List<string>(),
                KeyCode = p.KeyCode,
                IDCategory = p.IDCategory,
                IDProduct = p.IDProduct,
                CreateBy = p.CreateBy,
                CategoryName = p.Category.Name,
                Description = p.Description,
                Style = p.Style,
                Origin = p.Origin,
                IsActive = p.IsActive,
                Status = p.Status,
            }).ToList();
        }

        Task<ProductDetailsOnly> IProductDetailsService.GetByIDAsync(Guid ID)
        {
            throw new NotImplementedException();
        }

        Task<List<ProductDetails_IDName>> IProductDetailsService.GetProductDetails_IDNameAsync()
        {
            throw new NotImplementedException();
        }
    }
}

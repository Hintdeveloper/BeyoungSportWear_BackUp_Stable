using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.CartOptions;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Services.Implements
{
    public class CartOptionsService : ICartOptionsService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public CartOptionsService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(CartOptionsCreateVM request)
        {
            if (request.Quantity <= 0)
            {
                return false; 
            }
            var options = await _dbcontext.Options.FirstOrDefaultAsync(c => c.ID == request.IDOptions);
            var cart = await _dbcontext.Cart.FirstOrDefaultAsync(c => c.ID == request.IDCart);

            if (options != null && cart != null)
            {
                var existingCartOption = await _dbcontext.CartOptions
                    .FirstOrDefaultAsync(co => co.IDCart == request.IDCart && co.IDOptions == request.IDOptions);

                if (request.Quantity > options.StockQuantity)
                {
                    return false;
                }

                if (existingCartOption != null)
                {
                    if (existingCartOption.Quantity + request.Quantity > options.StockQuantity)
                    {
                        return false;
                    }

                    existingCartOption.Quantity += request.Quantity;
                    existingCartOption.TotalPrice = existingCartOption.Quantity * existingCartOption.UnitPrice;
                    existingCartOption.ModifiedDate = DateTime.Now;

                    _dbcontext.CartOptions.Update(existingCartOption);
                }
                else
                {
                    decimal totalPrice = request.Quantity * options.RetailPrice;

                    var cartproductdetails = new CartOptions
                    {
                        IDCart = cart.ID,
                        IDOptions = options.ID,
                        CreateBy = request.CreateBy,
                        Quantity = request.Quantity,
                        UnitPrice = options.RetailPrice,
                        TotalPrice = totalPrice,
                        CreateDate = DateTime.Now,
                        Status = 1
                    };

                    _dbcontext.CartOptions.Add(cartproductdetails);
                }

                await _dbcontext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<CartOptionsVM>> GetAllActiveAsync()
        {
            var Obj = await _dbcontext.CartOptions
                         .Where(c => c.Status == 1)
                         .ProjectTo<CartOptionsVM>(_mapper.ConfigurationProvider)
                         .ToListAsync();

            return Obj;
        }
        public async Task<List<CartOptionsVM>> GetAllAsync()
        {
            var Obj = await _dbcontext.CartOptions
                         .ProjectTo<CartOptionsVM>(_mapper.ConfigurationProvider)
                         .ToListAsync();

            return Obj;
        }
        public async Task<List<CartOptionsVM>> GetAllByCartIDAsync(string IDCart)
        {
            var cartOptions = await _dbcontext.CartOptions
                                                        .Where(co => co.IDCart == IDCart)
                                                        .OrderByDescending(co => co.CreateDate)
                                                        .Select(co => new CartOptionsVM
                                                        {
                                                            IDCart = co.IDCart,
                                                            IDOptions = co.IDOptions,
                                                            ProductName = co.Options.ProductDetails.Products.Name,
                                                            ImageURL = co.Options.ImageURL,
                                                            Quantity = co.Quantity,
                                                            SizeName = co.Options.Sizes.Name,
                                                            ColorName = co.Options.Colors.Name,
                                                            CreateDate = co.CreateDate,
                                                            UnitPrice = co.UnitPrice,
                                                            TotalPrice = co.Quantity * co.UnitPrice,
                                                        })
                                                        .ToListAsync();
            return cartOptions;
        } 
        public async Task<CartOptionsVM> GetByIDAsync(string IDCart, Guid? IDOptions)
        {
            var Obj = await _dbcontext.CartOptions
                          .Where(c => c.IDCart == IDCart && c.IDOptions == IDOptions)
                          .OrderByDescending(co => co.CreateDate)
                          .FirstOrDefaultAsync();

            if (Obj == null)
            {
                return null; 
            }

            var ObjVm = _mapper.Map<CartOptionsVM>(Obj);
            return ObjVm;
        }
        public async Task<Result> RemoveAsync(string IDCart, Guid? IDOptions)
        {
            try
            {
                var cartItem = await _dbcontext.CartOptions
                    .FirstOrDefaultAsync(c => c.IDCart == IDCart && c.IDOptions == IDOptions);

                if (cartItem != null)
                {
                    _dbcontext.CartOptions.Remove(cartItem);
                    await _dbcontext.SaveChangesAsync();

                    return new Result
                    {
                        Success = true,
                        ErrorMessage = "Cập nhật thành công"
                    };
                }
                else
                {
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = "Không tìm thấy sản phẩm trong giỏ hàng."
                    };
                }
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = $"Lỗi xảy ra: {ex.Message}"
                };
            }
        }
        public async Task<bool> UpdateAsync(string IDCart, Guid? IDOptions, CartOptionsUpdateVM request)
        {
            var cart = await _dbcontext.Cart
                .Include(c => c.CartOptions)
                .FirstOrDefaultAsync(c => c.ID == IDCart);

            if (cart == null)
            {
                return false;
            }

            var cartItem = cart.CartOptions
                .FirstOrDefault(ci => ci.IDOptions == IDOptions);

            if (cartItem == null)
            {
                return false;
            }

            var option = await _dbcontext.Options
                .FirstOrDefaultAsync(o => o.ID == IDOptions);

            if (option == null)
            {
                return false;
            }

            if (request.Quantity > option.StockQuantity) 
            {
                return false; 
            }

            cartItem.Quantity = request.Quantity;
            cartItem.UnitPrice = option.RetailPrice;
            cartItem.TotalPrice = cartItem.Quantity * cartItem.UnitPrice;

            await _dbcontext.SaveChangesAsync();

            return true;
        }
    }
}

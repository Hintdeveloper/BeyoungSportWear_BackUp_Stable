using AutoMapper;
using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.Product;
using DataAccessLayer.Application;
using DataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
namespace BusinessLogicLayer.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDBContext _dbcontext;
        private readonly IMapper _mapper;
        public ProductService(ApplicationDBContext ApplicationDBContext, IMapper mapper)
        {
            _dbcontext = ApplicationDBContext;
            _mapper = mapper;
        }
      
        public async Task<bool> CreateAsync(ProductCreateVM request)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var product = new Product
                    {
                        ID = Guid.NewGuid(),
                        Name = request.Name,
                        Description = request.Description,
                        Status = 1,
                        CreateBy = request.CreateBy,
                        CreateDate = DateTime.Now,
                    };

                    await _dbcontext.Product.AddAsync(product);
                    await _dbcontext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<List<ProductVM>> GetAllActiveAsync()
        {
            var Obj = await _dbcontext.Product.Where(b => b.Status == 1).ToListAsync();
            return _mapper.Map<List<ProductVM>>(Obj);
        }

        public async Task<List<ProductVM>> GetAllAsync()
        {
            var Obj = await _dbcontext.Product.ToListAsync();
            return _mapper.Map<List<ProductVM>>(Obj);
        }

        public async Task<ProductVM> GetByIDAsync(Guid ID)
        {
            var Obj = await _dbcontext.Product.FindAsync(ID);
            return _mapper.Map<ProductVM>(Obj);
        }

        public async Task<bool> RemoveAsync(Guid ID, string IDUserDelete)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var obj = await _dbcontext.Product.FirstOrDefaultAsync(c => c.ID == ID);

                    if (obj != null)
                    {
                        obj.Status = 0;
                        obj.DeleteDate = DateTime.Now;
                        obj.DeleteBy = IDUserDelete;

                        _dbcontext.Product.Attach(obj);
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

        public async Task<bool> UpdateAsync(Guid ID, ProductUpdateVM request)
        {
            var product = await _dbcontext.Product.FindAsync(ID);
            if (product == null)
            {
                return false;
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Status = request.Status;
            product.ModifiedBy = request.ModifiedBy;
            product.ModifiedDate = DateTime.Now;

            _dbcontext.Product.Update(product);

            await _dbcontext.SaveChangesAsync();
            return true;
        }

    }
}


using BusinessLogicLayer.Viewmodels.Product;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IProductService
    {
        public Task<List<ProductVM>> GetAllAsync();
        public Task<List<ProductVM>> GetAllActiveAsync();
        public Task<ProductVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(ProductCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserDelete);
        public Task<bool> UpdateAsync(Guid ID, ProductUpdateVM request);
    }
}

using BusinessLogicLayer.Viewmodels.Category;
using BusinessLogicLayer.Viewmodels.ProductDetails;

namespace BusinessLogicLayer.Services.Interface
{
    public interface ICategoryService
    {
        public Task<List<CategoryVM>> GetAllAsync();
        public Task<List<CategoryVM>> GetAllActiveAsync();
        public Task<CategoryVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(CategoryCreateVM request);
        public Task<bool> UpdateAsync(Guid ID, CategoryUpdateVM request);
        public Task<List<ProductDetailsVM>> GetProductsByCategoryAsync(Guid IDCategory);
        public Task<List<ProductDetailsVM>> GetProductsByPriceRangeAsync(Guid IDCategory, decimal minPrice, decimal maxPrice);
        public Task<bool> SetStatus(Guid ID);

    }
}

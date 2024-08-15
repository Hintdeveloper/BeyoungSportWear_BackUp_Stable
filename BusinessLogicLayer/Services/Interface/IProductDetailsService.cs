using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.CartOptions;
using BusinessLogicLayer.Viewmodels.Options;
using BusinessLogicLayer.Viewmodels.ProductDetails;

namespace BusinessLogicLayer.Services.Interface
{
    public interface IProductDetailsService : IActivatable
    {
        public Task<List<ProductDetailsVM>> GetAllAsync(int pageIndex, int pageSize);
        public Task<List<ProductDetailsVM>> GetAllActiveAsync(int pageIndex, int pageSize);
        public Task<ProductDetailsOnly> GetByIDAsync(Guid ID);
        public Task<ProductDetailsVM> GetByIDAsyncVer_1(Guid ID);
        public Task<List<OptionsVM>> GetOptionProductDetailsByIDAsync(Guid IDProductDetails);
        public Task<bool> CreateAsync(ProductDetailsCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserDelete);
        public Task<bool> UpdateAsync(Guid ID, ProductDetailsUpdateVM request);

        public IQueryable<ProductDetailsVM> Search(List<SearchCondition> conditions);
        public Task<ProductDetailUser> GetProductDetailInfo(Guid IDProductDetails, string size, string color);
        public Task<List<ProductDetails_IDName>> GetProductDetails_IDNameAsync();

        public Task<List<ProductDetailsVM>> GetProductsByPriceRangeAsync(/*Guid IDProduct,*/ decimal minPrice, decimal maxPrice);
    }
}

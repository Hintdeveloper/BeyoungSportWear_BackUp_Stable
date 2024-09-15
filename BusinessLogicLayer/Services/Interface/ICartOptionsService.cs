
using BusinessLogicLayer.Viewmodels;
using BusinessLogicLayer.Viewmodels.CartOptions;

namespace BusinessLogicLayer.Services.Interface
{
    public interface ICartOptionsService
    {
        public Task<List<CartOptionsVM>> GetAllAsync();
        public Task<List<CartOptionsVM>> GetAllActiveAsync();
        public Task<CartOptionsVM> GetByIDAsync(string IDCart, Guid? IDOptions);
        public Task<bool> CreateAsync(CartOptionsCreateVM request);
        public Task<Result> RemoveAsync(string IDCart, Guid? IDOptions);
        public Task<bool> UpdateAsync(string IDCart, Guid? IDOptions, CartOptionsUpdateVM request);
        public Task<List<CartOptionsVM>> GetAllByCartIDAsync(string IDCart);
    }
}

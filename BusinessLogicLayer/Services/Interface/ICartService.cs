using BusinessLogicLayer.Viewmodels.Cart;
namespace BusinessLogicLayer.Services.Interface
{
    public interface ICartService
    {
        public Task<List<CartVM>> GetAllAsync();
        public Task<List<CartVM>> GetAllActiveAsync();
        public Task<CartVM> GetByIDAsync(Guid ID);
        public Task<bool> CreateAsync(CartCreateVM request);
        public Task<bool> RemoveAsync(Guid ID, string IDUserDelete);
        public Task<bool> UpdateAsync(Guid ID, CartUpdateVM request);
        public Task<List<CartVM>> GetByUserIDAsync(string IDUser);
    }
}

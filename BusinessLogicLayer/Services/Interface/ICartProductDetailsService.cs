using BusinessLogicLayer.Viewmodels.CartProductDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Interface
{
    public interface ICartProductDetailsService
    {
        public Task<List<CartOptionsVM>> GetAllAsync();
        public Task<List<CartOptionsVM>> GetAllActiveAsync();
        public Task<CartOptionsVM> GetByIDAsync(Guid IDCart, Guid? IDOptions);
        public Task<bool> CreateAsync(CartOptionsCreateVM request);
        public Task<bool> RemoveAsync(Guid IDCart, Guid? IDOptions);
        public Task<bool> UpdateAsync(Guid IDCart, Guid? IDOptions, CartOptionsUpdateVM request);
        public Task<List<CartOptionsVM>> GetAllByCartIDAsync(Guid IDCart);
    }
}

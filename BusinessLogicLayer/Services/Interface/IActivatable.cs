
namespace BusinessLogicLayer.Services.Interface
{
    public interface IActivatable
    {
        Task<bool> UpdateIsActiveAsync(Guid ID, bool isActive);
    }
}

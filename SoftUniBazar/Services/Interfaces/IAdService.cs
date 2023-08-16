using SoftUniBazar.Models;

namespace SoftUniBazar.Services.Interfaces
{
    public interface IAdService
    {
        Task<IEnumerable<AdAllViewModel>> GetAllAdsAsync();
        Task AddAdAsync(AddAdViewModel model);
        Task<AddAdViewModel> GetNewAddAdModelAsync();
        Task<IEnumerable<CategoryViewModel>> GetAllCategoriesAsync();
        Task<IEnumerable<AdAllViewModel>> GetMyAdsAsync(string userId);
        Task<AdViewModel?> GetAdByIdAsync(int id);
        Task AddAdToCartAsync(string userId, AdViewModel ad);
        Task RemoveAdFromCartAsync(string userId, AdViewModel ad);
        Task<AddAdViewModel?> GetAdByIdForEditAsync(int id);
        Task EditAdAsync(AddAdViewModel model, int id);
    }
}

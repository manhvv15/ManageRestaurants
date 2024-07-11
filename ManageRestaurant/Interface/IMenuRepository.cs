using ManageRestaurant.Models;

namespace ManageRestaurant.Interface
{
    public interface IMenuRepository
    {
        Task<Menu> AddMenuAsync(Menu menu);
        Task<IEnumerable<Menu>> GetMenusAsync();
        Task<Menu> GetMenuByIdAsync(int id);
        Task<Menu> UpdateMenuAsync(Menu menu);
        Task<Menu> DeleteMenuAsync(int id);
    }
}

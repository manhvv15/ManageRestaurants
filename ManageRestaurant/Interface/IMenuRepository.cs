using ManageRestaurant.DTO;
using ManageRestaurant.Models;

namespace ManageRestaurant.Interface
{
    public interface IMenuRepository
    {
        Task<bool> AddMenuAsync(Menu menu);
        Task<List<Menu>> GetMenusAsync();
        Task<Menu> GetMenuByIdAsync(int id);
        Task<Menu> UpdateMenuAsync(int menuId, MenuDTO menu);
        Task<bool> DeleteMenuAsync(int id);
    }
}

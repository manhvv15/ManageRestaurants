using ManageRestaurant.DTO;
using ManageRestaurant.Interface;
using ManageRestaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageRestaurant.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly ManageRestaurantContext _context;
        public MenuRepository(IConfiguration configuration, ManageRestaurantContext context)
        {
            _context = context;
        }

        public async Task<List<Menu>> GetMenusAsync()
        {
            var menuItems = await _context.Menus.ToListAsync();
            return menuItems;
        }

        public async Task<Menu> GetMenuByIdAsync(int id)
        {
            var menuItem = await _context.Menus.FirstOrDefaultAsync(x => x.MenuId == id);
            return menuItem;
        }

        public async Task<bool> AddMenuAsync(Menu menu)
        {
            try
            {
                await _context.Menus.AddAsync(menu);
                await _context.SaveChangesAsync();
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Menu> UpdateMenuAsync(int menuId, MenuDTO menu)
        {
            var menuItem = await GetMenuByIdAsync(menuId);
            if (menuItem != null)
            {
                menuItem.Name = menu.Name;
                menuItem.Description = menu.Description;
                menuItem.Price = menu.Price;
                menuItem.ImageUrl = menu.ImageUrl;
                await _context.SaveChangesAsync();
            }
            return menuItem;
        }
        public async Task<bool> DeleteMenuAsync(int id)
        {
            var menuItem = await GetMenuByIdAsync(id);
            if (menuItem != null)
            {
                _context.Menus.Remove(menuItem);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}

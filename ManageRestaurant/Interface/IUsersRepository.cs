using ManageRestaurant.DTO;
using ManageRestaurant.Models;

namespace ManageRestaurant.Interface
{
    public interface IUsersRepository
    {
        Task<bool> RegisterUser(string username, string Email, string password);
        Task<User> LoginUser(string username, string password);
        Task<User> GetUserById(int userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(int userId,UserDTO user);
        Task<bool> DeleteAsync(int userId);
    }
}

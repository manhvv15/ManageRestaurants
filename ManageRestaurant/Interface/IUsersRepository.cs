using ManageRestaurant.Models;

namespace ManageRestaurant.Interface
{
    public interface IUsersRepository
    {
        Task<bool> RegisterUser(string username, string Email, string password);
        Task<User> LoginUser(string username, string password);
    }
}

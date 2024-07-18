namespace ManageRestaurant.DTO
{
    public class UserDTO
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public double? Balance { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
    }
}

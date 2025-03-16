using System.ComponentModel.DataAnnotations;


namespace E_commerce.Models;

public enum UserRole
{
    Admin,
    Seller,
    Buyer
}

public class UserModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    [EmailAddress] public required string Email { get; set; }
    [Phone] public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.Buyer; // Default role is "Buyer"
    
}

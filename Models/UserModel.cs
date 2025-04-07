using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text.Json.Serialization;


namespace E_commerce.Models;

public enum UserRole
{
    Admin,
    Seller,
    Buyer
}

public enum AccountStatus
{
    Active,
    Suspended,
}

public class UserModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    [EmailAddress] public required string Email { get; set; }
    [Phone] public required string PhoneNumber { get; set; }
    [JsonIgnore] public required string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.Buyer; // Default role is "Buyer"
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Active; // Default account status is "Active"

    public List<string>? Address
    {
        get; set;
    }
}

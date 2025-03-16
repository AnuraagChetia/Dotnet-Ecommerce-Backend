using System.ComponentModel.DataAnnotations;
using E_commerce.Models;

namespace E_commerce.Dtos;

public record class SignupDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    [EmailAddress] public required string Email { get; set; }
    [Phone] public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.Buyer;
};
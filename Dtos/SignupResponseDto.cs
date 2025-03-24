using System.ComponentModel.DataAnnotations;
using E_commerce.Models;

namespace E_commerce.Dtos;
public class SignupResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    [EmailAddress] public required string Email { get; set; }
    public required UserRole Role { get; set; }
    [Phone] public required string PhoneNumber { get; set; }
}

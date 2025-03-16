using System.ComponentModel.DataAnnotations;

namespace E_commerce.Dtos;

public record class LoginDto
(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_commerce.Data;
using E_commerce.Dtos;
using E_commerce.Models;
using E_commerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

/**
    Controller to update user information
    Controller for forget password using otp based verification either on email or phone number
    controller for reset password
    controller for user logout - destroys the token
    
*/

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController(EcommerceContext dbContext, PasswordService passwordService, IConfiguration configuration) : ControllerBase
    {
        private readonly EcommerceContext _dbContext = dbContext;
        private readonly PasswordService _passwordService = passwordService;

        private readonly IConfiguration _configuration = configuration;

        /**
            @notice This is the controller for signup functionality.
         */
        [HttpPost("signup")]
        async public Task<IActionResult> Signup(SignupDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Password = _passwordService.HashPassword(user.Password);

            await _dbContext.Users.AddAsync(new UserModel { Name = user.Name, Email = user.Email, Password = user.Password, Role = user.Role, PhoneNumber = user.PhoneNumber });

            await _dbContext.SaveChangesAsync();

            var response = new SignupResponseDto
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber
            };

            return CreatedAtAction(nameof(Signup), new { id = user.Id }, response);
        }
        /**
            @notice This is the controller for login functionality.
         */
        [HttpPost("login")]
        async public Task<IActionResult> Login(LoginDto loginDto)
        {
            // check if body is of valid type
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //Check if user exists in the database
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user is null) return Unauthorized(new { message = "Invalid email or password" });

            //Verify Password
            bool isPasswordValid = _passwordService.VerifyPassword(user.Password, loginDto.Password);
            if (!isPasswordValid) return Unauthorized(new { message = "Invalid Email or password" });

            var token = GenerateToken(user);

            return Ok(new { message = "Login Successful", token });
        }

        private string GenerateToken(UserModel user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

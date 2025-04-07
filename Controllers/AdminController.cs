using E_commerce.Data;
using E_commerce.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/**
    Controllers to get all products by a seller
    Controller to get details of a user along with their products
    Controller to suspend a user account
    Controller to delete a user account
    
*/
namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(EcommerceContext dbContext) : ControllerBase
    {
        private readonly EcommerceContext _dbContext = dbContext;

        /**
        * @notice Controller to get all products by a seller
        */
        [HttpGet("get-products-by-seller/{sellerId}")]
        [Authorize(Roles = "Admin")]
        async public Task<IActionResult> GetProductsBySeller(int sellerId)
        {
            // Get user from db
            var user = await _dbContext.Users.FindAsync(sellerId);
            Console.WriteLine("User: " + user);
            if (user == null || user.Role.ToString() != "Seller")
            {
                return NotFound(new { message = "Seller not found" });
            }
            // Ger all products by the seller from db but exclude the Seller data from the response
            var products = await _dbContext.Products.Where(product => product.SellerId == sellerId).Select(product => new ProductDto
            {
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductImgUrl = product.ProductImgUrl,
                SellerName = product.Seller!.Name
            }).ToListAsync();
            if (products.Count == 0)
            {
                return Ok(new { message = "No products found for thsi seller", products = products });
            }
            return Ok(products);
        }

        /**
        * @notice Controller to get details of a user along with their products
        */
        [HttpGet("get-user-details/{userId}")]
        [Authorize(Roles = "Admin")]
        async public Task<IActionResult> GetUserDetails(int userId)
        {
            return Ok();
        }

        /**
        * @notice Controller to suspend a user accout
        */
        [HttpPut("suspend-user/{userId}")]
        [Authorize(Roles = "Admin")]
        async public Task<IActionResult> SuspendUser(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            user.AccountStatus = Models.AccountStatus.Suspended;
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "User account suspended successfully" });
        }

        /**
        * @notice Controller to delete a usr account
        */
        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = "Admin")]
        async public Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            // delete the user
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "User account deleted successfully" });
        }
    }
}
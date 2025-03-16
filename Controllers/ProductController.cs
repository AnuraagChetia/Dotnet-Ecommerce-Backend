using E_commerce.Data;
using E_commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using E_commerce.Dtos;
using System.Security.Claims;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController(EcommerceContext dbContext) : ControllerBase
    {
        private readonly EcommerceContext _dbContext = dbContext;
        /** 
            @notice Controller to get all products
        */
        [HttpGet("get-products")]
        async public Task<IActionResult> GetProducts()
        {
            var products = await _dbContext.Products.Include(product => product.Seller).Select(product => new ProductDto
            {
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductImgUrl = product.ProductImgUrl,
                Category = product.Category,
                SellerName = product.Seller!.Name
            }).AsNoTracking().ToListAsync();
            return Ok(products);

        }
        /** 
            @notice Controller to get a specific products
        */
        [HttpGet("get-products/{id}")]
        async public Task<IActionResult> GetProducts(int id)
        {
            var products = await _dbContext.Products.Include(product => product.Seller).Where(product => product.Id == id).Select(product => new ProductDto
            {
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductImgUrl = product.ProductImgUrl,
                Category = product.Category,
                SellerName = product.Seller!.Name
            }).FirstOrDefaultAsync();
            return products == null ? NotFound() : Ok(products);
        }
        /** 
            @notice Controller to add products
        */
        [HttpPost("add-product")]
        [Authorize(Roles = "Seller")]
        async public Task<IActionResult> AddProduct(AddProductDto product)
        {
            //Check if payload is correct
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            ProductModel newProduct = new()
            {
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductImgUrl = product.ProductImgUrl,
                SellerId = sellerId,
                Category = product.Category
            };

            await _dbContext.Products.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(AddProduct), new { id = newProduct.Id }, newProduct);

        }
        /**
            @notice Controller to update products
        */
        [HttpPut("update-product/{id}")]
        [Authorize(Roles = "Admin,Seller")]
        async public Task<IActionResult> UpdateProduct(int id, UpdateProductDto updatedProduct)
        {
            //Check if payload is correct
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingProduct = await _dbContext.Products.FindAsync(id);

            if (existingProduct is null) return NotFound(new { message = "Product not found !" });

            // 🔹 Get Logged-in User ID from JWT Token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (userId != existingProduct.SellerId && User.IsInRole("Admin"))
            {
                return Unauthorized(new { message = "You are not authorized to update this product" });
            }


            existingProduct.ProductName = updatedProduct.ProductName;
            existingProduct.ProductPrice = updatedProduct.ProductPrice;
            existingProduct.ProductImgUrl = updatedProduct.ProductImgUrl;
            existingProduct.SellerId = userId;
            existingProduct.Category = updatedProduct.Category;

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Product updated successfully", product = existingProduct });
        }

        /**
           @notice Controller to delete products
        */
        [HttpDelete("delete-product/{id}")]
        [Authorize(Roles = "Admin,Seller")]
        async public Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _dbContext.Products.FindAsync(id);
            if (existingProduct is null) return NotFound(new { message = "Product doesn't exist" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            //Check if user is authorized to delete product
            if (userId != existingProduct.SellerId && User.IsInRole("Admin"))
            {
                return Unauthorized(new { message = "You are not authorized to update this product" });
            }

            await _dbContext.Products.Where(product => product.Id == id).ExecuteDeleteAsync();
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "Product deleted successfully", product = existingProduct });
        }

        /**
           @notice Controller to delete multiple products at once
        */
        [HttpDelete("delete-products")]
        [Authorize(Roles = "Admin")]
        async public Task<IActionResult> DeleteMultipleProducts(List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return BadRequest(new { message = "No product IDs provided" });
            }

            var existingProducts = await _dbContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (!existingProducts.Any())
            {
                return NotFound(new { message = "None of the specified products were found" });
            }

            await _dbContext.Products
                .Where(product => productIds.Contains(product.Id))
                .ExecuteDeleteAsync();

            return Ok(new
            {
                message = "Products deleted successfully",
                deletedCount = existingProducts.Count,
                deletedProducts = existingProducts
            });
        }

    }
}

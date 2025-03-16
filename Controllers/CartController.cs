using System.Security.Claims;
using E_commerce.Data;
using E_commerce.Dtos;
using E_commerce.Models;
using E_commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
/** 
    Add to cart
    delete from cart
    get cart
    buy cart with payment gateway integration
*/
namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(EcommerceContext dbContext, PaymentGatewayService paymentGateway) : ControllerBase
    {
        private readonly EcommerceContext _dbContext = dbContext;
        private readonly PaymentGatewayService _paymentGateway = paymentGateway;

        [HttpGet("get-cart")]
        async public Task<IActionResult> GetCart()
        {
            // get user id from claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // get cart from db
            var cart = await _dbContext.Carts.Where(cart => cart.UserId == userId).FirstOrDefaultAsync();

            if (cart == null)
            {
                return NotFound(new { message = "Cart not found" });
            }

            return Ok(cart);
        }

        [HttpPost("add-to-cart")]
        [Authorize]
        async public Task<IActionResult> AddToCart(AddOrRemoveProductToCartDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if product exists
            var existingProduct = await _dbContext.Products.FindAsync(productDto.ProductId);

            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // get user id from claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // get the cart from db
            var cart = await _dbContext.Carts.Where(cart => cart.UserId == userId).Include(cart => cart.Items).FirstOrDefaultAsync();
            if (cart == null)
            {
                // create a new cart
                cart = new CartModel
                {
                    UserId = userId,
                    Items = new List<CartItemModel>(),
                    TotalAmount = 0
                };
                await _dbContext.Carts.AddAsync(cart);
            }

            // update cart
            var existingProductInCart = cart.Items.Where(item => item.CartId == cart.Id).Where(item => item.ProductId == productDto.ProductId).FirstOrDefault();

            if (existingProductInCart == null)
            {
                cart.Items.Add(new CartItemModel
                {
                    CartId = cart.Id,
                    ProductId = productDto.ProductId,
                    Quantity = 1,
                    CartItemPrice = existingProduct.ProductPrice
                });
                cart.TotalAmount = cart.Items.Sum(item => item.CartItemPrice);
            }
            else
            {
                existingProductInCart.Quantity++;
                existingProductInCart.CartItemPrice = existingProductInCart.Quantity * existingProduct.ProductPrice;
                cart.TotalAmount = cart.Items.Sum(item => item.CartItemPrice);
            }

            // save changes
            await _dbContext.SaveChangesAsync();

            return Ok(cart);

        }

        [HttpDelete("delete-from-cart")]
        [Authorize]
        async public Task<IActionResult> DeleteFromCart(AddOrRemoveProductToCartDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check if product exists
            var existingProduct = await _dbContext.Products.FindAsync(productDto.ProductId);

            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // get user id from claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // get the cart from db
            var cart = await _dbContext.Carts.Where(cart => cart.UserId == userId).Include(cart => cart.Items).FirstOrDefaultAsync();
            if (cart == null)
            {
                // create a new cart
                cart = new CartModel
                {
                    UserId = userId,
                    Items = new List<CartItemModel>(),
                    TotalAmount = 0
                };
                await _dbContext.Carts.AddAsync(cart);
            }

            //update the cart
            var existingProductInCart = cart.Items.Where(item => item.ProductId == productDto.ProductId).FirstOrDefault();
            if (existingProductInCart == null)
            {
                return NotFound(new { message = "Product not found in cart", cart });
            }
            existingProductInCart.Quantity--;
            // if quantity is 0, remove the product from cart
            if (existingProductInCart.Quantity == 0)
            {
                cart.Items.Remove(existingProductInCart);
                // return Ok(new { message = "inside here" });
            }
            else
            {
                existingProductInCart.CartItemPrice = existingProductInCart.Quantity * existingProduct.ProductPrice;
            }
            cart.TotalAmount = cart.Items.Sum(item => item.CartItemPrice);

            //save changes
            await _dbContext.SaveChangesAsync();
            return Ok(cart);
        }

        [HttpPost("buy-cart")]
        async public Task<IActionResult> BuyCart()
        {
            // get user id from claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            // get cart from db
            var cart = await _dbContext.Carts.Where(cart => cart.UserId == userId).Include(cart => cart.Items).FirstOrDefaultAsync();
            // check if cart is empty
            if (cart != null && cart.Items.Count == 0) return BadRequest(new { message = "Cart is empty" });
            // // payment gateway integration

            var order = _paymentGateway.ProcessPayment(400, "INR");

            // pay against the order id


            // if payment is successful, empty the cart (simulation)
            if (order != null)
            {
                cart!.Items.Clear();
                cart.TotalAmount = 0;
                await _dbContext.SaveChangesAsync();
                return Ok(new { message = "Payment successful" });
            }
            return BadRequest(new { message = "Payment failed" });

        }
    }
}

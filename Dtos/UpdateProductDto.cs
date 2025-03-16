using E_commerce.Models;

namespace E_commerce.Dtos
{
    public class UpdateProductDto
    {
        public required string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public required string ProductImgUrl { get; set; }
        public ProductCategory Category { get; set; }
        
    }
}
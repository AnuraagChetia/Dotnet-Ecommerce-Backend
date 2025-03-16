namespace E_commerce.Models;

public enum ProductCategory{
    Electronics,
    Fashion,
    Home,
    Toys,
    Books,
    Other
}

public class ProductModel
{
    public int Id { get; set; }
    public required string ProductName { get; set; }

    public required int ProductPrice { get; set; }

    public required string ProductImgUrl { get; set; }

    public required ProductCategory Category { get; set; }

    public required int SellerId { get; set; }

    public UserModel? Seller { get; set; } // Navigation property for the seller

}
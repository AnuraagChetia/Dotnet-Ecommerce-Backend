using System.Text.Json.Serialization;

namespace E_commerce.Models;

public class CartItemModel
{
    public int Id { get; set; }

    public int CartId { get; set; }

    [JsonIgnore]
    public CartModel? Cart { get; set; }

    public int ProductId { get; set; }

    [JsonIgnore]
    public ProductModel? Product { get; set; }

    public int Quantity { get; set; }

    public decimal CartItemPrice { get; set; } = 0;
}
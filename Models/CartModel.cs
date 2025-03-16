using System.Text.Json.Serialization;

namespace E_commerce.Models;

public class CartModel
{
    public int Id { get; set; }

    public List<CartItemModel> Items { get; set; } = [];

    public decimal TotalAmount { get; set; }

    public int UserId { get; set; }

    [JsonIgnore]
    public UserModel? User { get; set; }

}
using Razorpay.Api;

namespace E_commerce.Services;

public class PaymentGatewayService
{
    public Order ProcessPayment(decimal amount, string currency)
    {
        //process payment 
        string key = "rzp_test_WPPtRMH62T2trC";
        string secret = "CDESFDbyUVmbQK1JFbUMCUQb";
        RazorpayClient client = new(key, secret);

        Dictionary<string, object> input = new Dictionary<string, object>
        {
            { "amount", amount * 100 }, // Amount is in currency subunits. Default currency is INR. Hence, 50000 refers to 50000 paise
            { "currency", currency }
        };
        Order order = client.Order.Create(input);

        Console.WriteLine(order.Attributes["id"]);
        return order;    
    }
}
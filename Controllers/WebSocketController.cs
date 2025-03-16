using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using E_commerce.Data;
using Microsoft.AspNetCore.Mvc;


namespace E_commerce.Controllers;

public class WebSocketController(EcommerceContext dbContext) : ControllerBase
{
    private readonly EcommerceContext _dbContext = dbContext;
    [Route("/ws")]
    async public Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _dbContext.Users.FindAsync(userId);
            Console.WriteLine(userId);
            
            await Echo(webSocket, user.Name);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    private static async Task Echo(WebSocket webSocket, String userName)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            string responseMessage = $"Hi {userName}, you said: {Encoding.UTF8.GetString(buffer)}";

            byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);

            await webSocket.SendAsync(
                new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
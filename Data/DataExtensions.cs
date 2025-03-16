using Microsoft.EntityFrameworkCore;

namespace E_commerce.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceContext>();
        await dbContext.Database.MigrateAsync();
    }
}
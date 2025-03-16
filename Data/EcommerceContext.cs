using E_commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Data;

public class EcommerceContext(DbContextOptions<EcommerceContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users => Set<UserModel>();

    public DbSet<ProductModel> Products => Set<ProductModel>();

    public DbSet<CartModel> Carts => Set<CartModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>().HasIndex(user => user.Email).IsUnique(); // Email should be unique

        modelBuilder.Entity<UserModel>()
       .Property(u => u.Role)
       .HasConversion<string>(); // Stores enum as a string


    }

}


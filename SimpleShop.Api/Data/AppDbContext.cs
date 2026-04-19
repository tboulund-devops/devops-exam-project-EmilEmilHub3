using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Models;

namespace SimpleShop.Api.Data;

/// <summary>
/// Represents the Entity Framework Core database context for the SimpleShop application.
/// Responsible for database access, entity tracking,
/// table mappings, and relationship configuration.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">
    /// Database context options provided through dependency injection.
    /// </param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the Products table.
    /// Contains all webshop products.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Gets the Users table.
    /// Contains registered customer accounts.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the CartItems table.
    /// Contains products currently added to user carts.
    /// </summary>
    public DbSet<CartItem> CartItems => Set<CartItem>();

    /// <summary>
    /// Gets the Orders table.
    /// Contains completed customer orders.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>
    /// Gets the OrderLines table.
    /// Contains products belonging to specific orders.
    /// </summary>
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    /// <summary>
    /// Configures entities, relationships, indexes,
    /// required fields, and database precision rules.
    /// </summary>
    /// <param name="modelBuilder">
    /// Used by Entity Framework Core to configure models.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Product Configuration

        modelBuilder.Entity<Product>(e =>
        {
            // Product name is required and limited in length.
            e.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Store prices with two decimal places.
            e.Property(p => p.Price)
                .HasPrecision(10, 2);
        });

        #endregion

        #region User Configuration

        modelBuilder.Entity<User>(e =>
        {
            // Username is required with max length validation.
            e.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            // Email is required and limited in length.
            e.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            // Password hash must always be present.
            e.Property(u => u.PasswordHash)
                .IsRequired();

            // Prevent duplicate email addresses.
            e.HasIndex(u => u.Email)
                .IsUnique();
        });

        #endregion

        #region CartItem Configuration

        modelBuilder.Entity<CartItem>(e =>
        {
            // Quantity is mandatory.
            e.Property(c => c.Quantity)
                .IsRequired();

            // One user can have many cart items.
            e.HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product cannot be deleted if referenced in cart.
            e.HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Order Configuration

        modelBuilder.Entity<Order>(e =>
        {
            // One user can have many orders.
            e.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        #endregion

        #region OrderLine Configuration

        modelBuilder.Entity<OrderLine>(e =>
        {
            // Quantity is mandatory.
            e.Property(ol => ol.Quantity)
                .IsRequired();

            // Store order prices with two decimal places.
            e.Property(ol => ol.UnitPrice)
                .HasPrecision(10, 2);

            // One order contains many order lines.
            e.HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product cannot be deleted if used in orders.
            e.HasOne(ol => ol.Product)
                .WithMany()
                .HasForeignKey(ol => ol.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion
    }
}
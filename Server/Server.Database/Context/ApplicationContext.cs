using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Domain.Entities;

namespace Server.Database.Context;

public class ApplicationContext : DbContext
{
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderMenuItem> OrderMenuItems => Set<OrderMenuItem>();
    public DbSet<User> Users => Set<User>();

    private readonly ILogger<ApplicationContext>? _logger;

    public ApplicationContext(
        DbContextOptions<ApplicationContext> dbContextOptions,
        ILogger<ApplicationContext>? logger = null)
        : base(dbContextOptions)
    {
        _logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.LogTo(LogMessage, LogLevel.Information);
    }

    private void LogMessage(string message)
    {
        _logger?.LogInformation(message);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region OrderMenuItem

        modelBuilder.Entity<OrderMenuItem>()
            .HasKey(x => new { x.OrderId, x.MenuItemId });

        modelBuilder.Entity<OrderMenuItem>()
            .HasOne(x => x.Order)
            .WithMany(x => x.OrderMenuItems)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderMenuItem>()
            .HasOne(x => x.MenuItem)
            .WithMany(x => x.OrderMenuItems)
            .HasForeignKey(x => x.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region User

        modelBuilder.Entity<User>()
            .HasIndex(x => x.NormalizedName)
            .IsUnique();

        #endregion
    }
}
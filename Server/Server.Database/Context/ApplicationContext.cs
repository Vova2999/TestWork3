using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Domain.Entities;

namespace Server.Database.Context;

public class ApplicationContext : DbContext
{
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
        #region User

        modelBuilder.Entity<User>()
            .HasIndex(x => x.NormalizedName)
            .IsUnique();

        #endregion
    }
}
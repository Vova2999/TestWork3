// ReSharper disable UnusedType.Global

using Microsoft.EntityFrameworkCore.Design;
using Server.Database.Context;
using Server.Database.Helpers;

namespace Server.Context;

public class ApplicationContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var options = ApplicationContextHelper.BuildOptions(connectionString);

        return new ApplicationContext(options);
    }
}
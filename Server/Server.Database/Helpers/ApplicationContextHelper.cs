using Microsoft.EntityFrameworkCore;
using Server.Database.Context;

namespace Server.Database.Helpers;

public static class ApplicationContextHelper
{
    public static DbContextOptions<ApplicationContext> BuildOptions(string connectionString)
    {
        return new DbContextOptionsBuilder<ApplicationContext>().UseNpgsql(connectionString).Options;
    }
}
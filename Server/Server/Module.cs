using Microsoft.AspNetCore.Identity;
using Server.Database.Context.Factory;
using Server.Database.Helpers;
using Server.Domain.Entities;
using Server.Extensions;
using Server.Services.Managers;
using Server.Services.Migrations;
using Server.Services.Startup;
using Server.Services.Stores;

namespace Server;

public static class Module
{
    public static void RegisterDependencies(IServiceCollection service, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        service.AddSingletonOptions<ApplicationContextStartupOptions>(configuration);

        service.AddSingleton<IApplicationContextFactory, ApplicationContextFactory>();
        service.AddSingleton(_ => ApplicationContextHelper.BuildOptions(connectionString));

        service.AddScoped<ApplicationContextUserManager>();
        service.AddScoped<ApplicationContextSignInManager>();

        service.AddScoped<UserManager<User>, ApplicationContextUserManager>();
        service.AddScoped<SignInManager<User>, ApplicationContextSignInManager>();

        service.AddScoped<IApplicationContextUserStore, ApplicationContextUserStore>();
        service.AddScoped<IApplicationContextStartupService, ApplicationContextStartupService>();
        service.AddScoped<IApplicationContextMigrationsService, ApplicationContextMigrationsService>();

        service.AddIdentityCore<User>().AddUserStore<ApplicationContextUserStore>();
    }
}
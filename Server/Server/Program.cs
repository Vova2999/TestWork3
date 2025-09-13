using Microsoft.AspNetCore.Authentication;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using NSwag.Annotations;
using Server.Authentication;
using Server.Common.Extensions;
using Server.Exceptions;
using Server.Services.Migrations;
using Server.Services.Startup;

namespace Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        LogManager.Configuration = new NLogLoggingConfiguration(
            builder.Configuration.GetSection("NLog"));

        Module.RegisterDependencies(builder.Services, builder.Configuration);

        builder.Services.AddAuthentication(Constants.Basic)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(Constants.Basic, null);

        builder.Services
            .AddControllers(options =>
                options.AllowEmptyInputInBodyModelBinding = true)
            .ConfigureApiBehaviorOptions(options =>
                options.SuppressModelStateInvalidFilter = true);

        builder.Services
            .AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "Server";
                settings.Title = "Server";
                settings.Description = "API documentation";
            });

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionsHandler>();

        builder.Host.UseNLog();

        var app = builder.Build();

        await ApplyMigrationsAsync(app);
        InitializeUsersAsync(app).FireAndForgetSafeAsync();

        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();

            app.MapGet("/", [OpenApiIgnore] () => Results.Redirect("/swagger"));
        }

        app.MapControllers();

        await app.RunAsync();
    }

    private static async Task ApplyMigrationsAsync(IHost app)
    {
        using var scope = app.Services.CreateScope();

        var applicationContextMigrationsService = scope.ServiceProvider
            .GetRequiredService<IApplicationContextMigrationsService>();

        await applicationContextMigrationsService.ApplyMigrationsAsync();
    }

    private static async Task InitializeUsersAsync(IHost app)
    {
        using var scope = app.Services.CreateScope();

        var applicationContextStartupService = scope.ServiceProvider
            .GetRequiredService<IApplicationContextStartupService>();

        await applicationContextStartupService.InitializeUsersAsync();
    }
}
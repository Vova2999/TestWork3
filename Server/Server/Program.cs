using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using NSwag;
using NSwag.Annotations;
using NSwag.Generation.Processors.Security;
using Server.Authentication;
using Server.Common.Extensions;
using Server.Converters;
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
                options.SuppressModelStateInvalidFilter = true)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new CommandDtoJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new CommandResultDtoJsonConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 5;
            options.Password.RequiredUniqueChars = 1;
        });

        builder.Services
            .AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "Server";
                settings.Title = "Server";
                settings.Description = "API documentation";

                settings.AddSecurity(
                    Constants.Basic,
                    [],
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.Http,
                        Scheme = Constants.Basic,
                        Description = "Enter your username and password for Basic Authentication"
                    });

                settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(Constants.Basic));
            });

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionsHandler>();

        builder.Host.UseNLog();

        var app = builder.Build();

        await ApplyMigrationsAsync(app);
        InitializeUsersAndDataAsync(app).FireAndForgetSafeAsync();

        app.UseExceptionHandler();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();

            app.MapGet("/", [OpenApiIgnore]() => Results.Redirect("/swagger"));
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

    private static async Task InitializeUsersAndDataAsync(IHost app)
    {
        using var scope = app.Services.CreateScope();

        var applicationContextStartupService = scope.ServiceProvider
            .GetRequiredService<IApplicationContextStartupService>();

        await applicationContextStartupService.InitializeUsersAsync();
        await applicationContextStartupService.CreateOtherDataAsync();
    }
}
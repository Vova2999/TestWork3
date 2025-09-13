namespace Server.Services.Migrations;

public interface IApplicationContextMigrationsService
{
    Task ApplyMigrationsAsync();
}
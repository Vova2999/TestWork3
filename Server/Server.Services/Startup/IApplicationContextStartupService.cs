namespace Server.Services.Startup;

public interface IApplicationContextStartupService
{
    Task InitializeUsersAsync();
}
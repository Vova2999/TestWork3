using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Common.Extensions;
using Server.Domain.Entities;
using Server.Services.Managers;

namespace Server.Services.Startup;

public class ApplicationContextStartupService : IApplicationContextStartupService
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContextStartupOptions _applicationContextStartupOptions;
    private readonly ILogger<ApplicationContextStartupService> _logger;

    public ApplicationContextStartupService(
        ApplicationContextUserManager applicationContextUserManager,
        ApplicationContextStartupOptions applicationContextStartupOptions,
        ILogger<ApplicationContextStartupService> logger)
    {
        _applicationContextUserManager = applicationContextUserManager;
        _applicationContextStartupOptions = applicationContextStartupOptions;
        _logger = logger;
    }

    public async Task InitializeUsersAsync()
    {
        try
        {
            await InitializeUsersInternalAsync();
        }
        catch (Exception exception)
        {
            const string message = "Error on initialize users";

            _logger.LogCritical(exception, message);
            throw new Exception(message, exception);
        }
    }

    private async Task InitializeUsersInternalAsync()
    {
        var login = _applicationContextStartupOptions.InitializeUserLogin;
        var password = _applicationContextStartupOptions.InitializeUserPassword;
        if (login.IsNullOrEmpty() || password.IsNullOrEmpty())
            return;

        var hasUsers = await _applicationContextUserManager.Users.AnyAsync();
        if (hasUsers)
            return;

        var user = new User { Id = Guid.NewGuid(), Name = login };
        await _applicationContextUserManager.CreateAsync(user, password);
    }
}
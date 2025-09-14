using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Common.Extensions;
using Server.Database.Context.Factory;
using Server.Domain.Entities;
using Server.Services.Managers;

namespace Server.Services.Startup;

public class ApplicationContextStartupService : IApplicationContextStartupService
{
    private readonly IApplicationContextFactory _applicationContextFactory;
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContextStartupOptions _applicationContextStartupOptions;
    private readonly ILogger<ApplicationContextStartupService> _logger;

    public ApplicationContextStartupService(
        IApplicationContextFactory applicationContextFactory,
        ApplicationContextUserManager applicationContextUserManager,
        ApplicationContextStartupOptions applicationContextStartupOptions,
        ILogger<ApplicationContextStartupService> logger)
    {
        _applicationContextFactory = applicationContextFactory;
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

    public async Task CreateOtherDataAsync()
    {
        try
        {
            await CreateOtherDataInternalAsync();
        }
        catch (Exception exception)
        {
            const string message = "Error on create data";

            _logger.LogCritical(exception, message);
            throw new Exception(message, exception);
        }
    }

    private async Task CreateOtherDataInternalAsync()
    {
        await using var context = _applicationContextFactory.Create();

        if (await context.MenuItems.AnyAsync() || await context.Orders.AnyAsync())
            return;

        context.MenuItems.AddRange(GetConstantMenuItems());
        context.Orders.AddRange(GetConstantOrders());

        await context.SaveChangesAsync();
    }

    private static MenuItem[] GetConstantMenuItems()
    {
        return
        [
            new MenuItem
            {
                Id = "5979224",
                Article = "A1004292",
                Name = "Каша гречневая",
                Price = 50,
                IsWeighted = false,
                FullPath = "ПРОИЗВОДСТВО\\Гарниры",
                Barcodes = ["57890975627974236429"]
            },
            new MenuItem
            {
                Id = "9084246",
                Article = "A1004293",
                Name = "Конфеты Коровка",
                Price = 300,
                IsWeighted = true,
                FullPath = "ДЕСЕРТЫ\\Развес",
                Barcodes = []
            }
        ];
    }

    private static Order[] GetConstantOrders()
    {
        return
        [
            new Order
            {
                Id = Guid.Parse("62137983-1117-4D10-87C1-EF40A4348250"),
                OrderMenuItems =
                [
                    new OrderMenuItem { MenuItemId = "5979224", Quantity = 1 },
                    new OrderMenuItem { MenuItemId = "9084246", Quantity = 0.408M }
                ]
            }
        ];
    }
}
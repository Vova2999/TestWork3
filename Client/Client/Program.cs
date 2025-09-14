using Client.Api.Clients.Menu;
using Client.Api.Token;
using Client.Common;
using Client.Domain.Dtos;
using Client.Services.Menu;

namespace Client;

public static class Program
{
    private static readonly IAuthenticatorProvider AuthenticatorProvider;
    private static readonly IMenuService MenuService;

    static Program()
    {
        AuthenticatorProvider = new BasicAuthenticatorProvider();
        MenuService = new MenuService(new MenuClient(AuthenticatorProvider, Constants.ServerAddress, Constants.RequestTimeout));
    }

    public static async Task Main()
    {
        await AuthenticatorProvider.LoginAsync(new LoginDto { Login = "admin", Password = "admin" });

        var getMenuResult = await MenuService.GetMenuAsync(true);
        if (getMenuResult?.MenuItems.Any() != true)
        {
            Console.WriteLine("Ошибка при получении блюд с сервера");
            Console.ReadKey();

            return;
        }

        Console.WriteLine($"С сервера получены блюда с id = {string.Join(", ", getMenuResult.MenuItems.Select(menuItem => menuItem.Id))}");

        await MenuService.SendOrderAsync(
            Guid.Parse("62137983-1117-4D10-87C1-EF40A4348250"),
            [
                new OrderMenuItemDto { Id = "5979224", Quantity = 1 },
                new OrderMenuItemDto { Id = "9084246", Quantity = 0.408m }
            ]);

        Console.WriteLine("На сервер успешно отправлен заказ");
        Console.ReadKey();
    }
}
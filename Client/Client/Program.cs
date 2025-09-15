using Client.Api.Clients.Menu;
using Client.Api.Token;
using Client.Common;
using Client.Domain.Dtos;
using Client.Services.Menu;
using Client.Services.SmsTest;
using Grpc.Net.Client;
using Sms.Test;
using SmsTestService = Client.Services.SmsTest.SmsTestService;
using GrpcSmsTestService = Sms.Test.SmsTestService.SmsTestServiceClient;

namespace Client;

public static class Program
{
    private static readonly IAuthenticatorProvider AuthenticatorProvider;
    private static readonly IMenuService MenuService;
    private static readonly ISmsTestService SmsTestService;

    static Program()
    {
        AuthenticatorProvider = new BasicAuthenticatorProvider();
        MenuService = new MenuService(new MenuClient(AuthenticatorProvider, Constants.ServerAddress, Constants.RequestTimeout));
        SmsTestService = new SmsTestService(new GrpcSmsTestService(GrpcChannel.ForAddress(Constants.ServerAddress)));
    }

    public static async Task Main()
    {
        try
        {
            await UseHttp();
            await UseGrpc();
        }
        catch (Exception exception)
        {
            Console.WriteLine("Произошла ошибка");
            Console.WriteLine(exception);
        }

        Console.ReadKey();
    }

    private static async Task UseHttp()
    {
        Console.WriteLine();
        Console.WriteLine("Запросы через http");

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
    }

    private static async Task UseGrpc()
    {
        Console.WriteLine();
        Console.WriteLine("Запросы через grpc");

        var getMenuResult = await SmsTestService.GetMenuAsync(true);
        if (getMenuResult.MenuItems.Any() != true)
        {
            Console.WriteLine("Ошибка при получении блюд с сервера");
            Console.ReadKey();

            return;
        }

        Console.WriteLine($"С сервера получены блюда с id = {string.Join(", ", getMenuResult.MenuItems.Select(menuItem => menuItem.Id))}");

        await SmsTestService.SendOrderAsync(
            Guid.Parse("62137983-1117-4D10-87C1-EF40A4348250"),
            [
                new OrderItem { Id = "5979224", Quantity = 1 },
                new OrderItem { Id = "9084246", Quantity = 0.408 }
            ]);

        Console.WriteLine("На сервер успешно отправлен заказ");
    }
}
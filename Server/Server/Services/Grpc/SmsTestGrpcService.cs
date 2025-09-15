using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Server.Common.Extensions;
using Server.Database.Context.Factory;
using Server.Domain.Grpc.Extensions;
using Sms.Test;

namespace Server.Services.Grpc;

public class SmsTestGrpcService : SmsTestService.SmsTestServiceBase
{
    private readonly IApplicationContextFactory _applicationContextFactory;

    public SmsTestGrpcService(IApplicationContextFactory applicationContextFactory)
    {
        _applicationContextFactory = applicationContextFactory;
    }

    public override async Task<GetMenuResponse> GetMenu(BoolValue request, ServerCallContext serverCallContext)
    {
        await using var context = _applicationContextFactory.Create();

        var menuItems = await context.MenuItems
            .AsNoTracking()
            .Select(menuItem => menuItem.ToGrpcDto())
            .ToArrayAsync();

        if (!request.Value)
            menuItems.ForEach(menuItem => menuItem.Price = 0);

        return new GetMenuResponse { Success = true, MenuItems = { menuItems } };
    }

    public override Task<SendOrderResponse> SendOrder(Order request, ServerCallContext serverCallContext)
    {
        return Task.FromResult(new SendOrderResponse { Success = true });
    }
}
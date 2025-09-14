using Microsoft.EntityFrameworkCore;
using Server.Common.Extensions;
using Server.Database.Context.Factory;
using Server.Domain.Dtos.CommandParameters;
using Server.Domain.Dtos.CommandResultData;
using Server.Domain.Extensions;

namespace Server.Services.CommandParameters.Handlers;

public class GetMenuCommandParametersHandler : CommandParametersHandlerBase<GetMenuCommandParametersDto, GetMenuCommandResultDataDto>
{
    private readonly IApplicationContextFactory _applicationContextFactory;

    public GetMenuCommandParametersHandler(IApplicationContextFactory applicationContextFactory)
    {
        _applicationContextFactory = applicationContextFactory;
    }

    protected override async Task<GetMenuCommandResultDataDto> HandleInternalAsync(GetMenuCommandParametersDto commandParameters)
    {
        await using var context = _applicationContextFactory.Create();

        var menuItems = await context.MenuItems
            .AsNoTracking()
            .Select(menuItem => menuItem.ToDto())
            .ToArrayAsync();

        if (!commandParameters.WithPrice)
            menuItems.ForEach(menuItem => menuItem.Price = null);

        return new GetMenuCommandResultDataDto { MenuItems = menuItems };
    }
}
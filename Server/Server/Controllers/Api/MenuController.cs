using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Dtos;
using Server.Domain.Dtos.CommandResultData;
using Server.Domain.Extensions;

namespace Server.Controllers.Api;

[AllowAnonymous]
[ApiController]
[Route("api/menu")]
[Produces(MediaTypeNames.Application.Json)]
public class MenuController : ControllerBase
{
    [HttpPost]
    public CommandResultDto Post([FromBody] CommandDto command)
    {
        return new GetMenuCommandResultDataDto { MenuItems = new[] { new MenuItemDto { Id = "123" } } }.ToSuccessResultDto();
    }
}
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Dtos;
using Server.Services.Command;

namespace Server.Controllers.Api;

[AllowAnonymous]
[ApiController]
[Route("api/menu")]
[Produces(MediaTypeNames.Application.Json)]
public class MenuController : ControllerBase
{
    private readonly ICommandHandler _commandHandler;

    public MenuController(ICommandHandler commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [HttpPost]
    public async Task<CommandResultDto> PostAsync([FromBody] CommandDto? command)
    {
        return await _commandHandler.HandleAsync(command);
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers.Api;

[AllowAnonymous]
[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public void Get()
    {
    }
}
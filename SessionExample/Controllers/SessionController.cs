using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SessionExample.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionController(ILogger<SessionController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet(Name = "GetSession")]
    public IActionResult Get()
    {
        var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var username = _httpContextAccessor.HttpContext.Session.GetString(userId.ToString());
        _logger.LogInformation($"Session: {userId} Value: {username}");
        return Ok(new { username = username });
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SessionExample.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginController(ILogger<LoginController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost(Name = "PostLogin")]
    public IActionResult PostLogin([FromBody] LoginRequest request)
    {
        if (!string.IsNullOrEmpty(request.username))
        {
            var userId = Guid.NewGuid();
            var token = GenerateToken(userId);

            SetSession(userId, request.username);

            return Ok(new
            {
                token = token.token,
                expiration = token.expiration
            });
        }
        return Unauthorized();
    }

    private (string token, DateTime expiration) GenerateToken(Guid userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("f35dc933-eec5-4c53-822a-355f893b6ddc");
        var tokenDurationInDays = 1;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
            Expires = DateTime.UtcNow.AddDays(tokenDurationInDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return (tokenString, token.ValidTo);
    }

    private void SetSession(Guid userId, string username)
    {
        HttpContext.Session.SetString(userId.ToString(), username);
        _logger.LogInformation($"Session: {userId} Value: {username}");
    }
}

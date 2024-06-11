using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(SignInManager<AppUser> signInManager, ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }


    /// <summary>
    /// Get all roles for current user
    /// </summary>
    /// <response code="200">Returns the role items</response>
    /// <response code="400">If the item is null</response>
    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public IActionResult GetRoles()
    {
        var user = HttpContext.User;
        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            var identity = (ClaimsIdentity)user.Identity;
            var roles = identity.FindAll(identity.RoleClaimType)
                .Select(c => new
                {
                    c.Issuer,
                    c.OriginalIssuer,
                    c.Type,
                    c.Value,
                    c.ValueType
                });

            return new JsonResult(roles);
        }

        return Unauthorized();
    }

    /// <summary>
    /// Logout current user
    /// </summary>
    /// <param name="empty"></param>
    /// <returns></returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] object empty)
    {
        if (empty is not null)
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        return Unauthorized();
    }        
}

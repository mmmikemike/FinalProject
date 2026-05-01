using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Security;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(DemoJwtTokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (!DemoAccountStore.TryValidate(request.UserName, request.Password, out var account))
        {
            return Unauthorized("Username or password is invalid.");
        }

        var (token, expiresAt) = tokenService.CreateToken(account);

        return Ok(new LoginResponse(
            account.UserName,
            account.DisplayName,
            account.Role,
            account.TenantId,
            token,
            expiresAt));
    }
}

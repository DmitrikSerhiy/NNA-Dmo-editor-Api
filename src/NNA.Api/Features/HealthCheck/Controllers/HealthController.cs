using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NNA.Api.Attributes;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Features.HealthCheck.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class HealthController : ControllerBase {
    private readonly IUserRepository _repository;

    public HealthController(IUserRepository repository) {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("")]
    public ActionResult<string> Check() {
        return Ok("App is ok");
    }


    [HttpGet]
    [AllowAnonymous]
    [Route("database")]
    public async Task<ActionResult<string>> CheckDb(CancellationToken cancellationToken) {
        var user = await _repository.FirstUserAsync(cancellationToken);
        return Ok($"Db is ok. First user: {user.Email}");
    }

    [HttpGet]
    [SuperUserAuthorize]
    [Route("security")]
    public IActionResult CheckToken() {
        return new JsonResult(new { message = "Token is valid" });
    }
}
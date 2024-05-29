using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class SecureDataController : ControllerBase
{
    [HttpGet]
    [Authorize] // Protect this endpoint
    public IActionResult GetSecureData()
    {
        return Ok("This is secure data.");
    }
}

using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DelegatedController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] dynamic body)
    {
        return Ok(body);
    }
}
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DelegatedController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] dynamic body)
    {
        using var httpClient = new HttpClient();
        var result = await httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me")
            {
                Headers =
                {
                    Authorization = "Bearer " + body.access_token
                }
            });

        return Ok(result);
    }
}
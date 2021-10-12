using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DelegatedController : ControllerBase
{
    public class Request
    {
        [JsonPropertyName("api_url")] public string ApiUrl { get; set; }
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Request body)
    {
        using var httpClient = new HttpClient();
        var result = await httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, body.ApiUrl)
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", body.AccessToken)
                }
            });

        return Ok(await result.Content.ReadAsStringAsync());
    }
}
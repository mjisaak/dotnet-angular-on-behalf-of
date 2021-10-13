using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using backend.Services;
using backend.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DelegatedController : ControllerBase
{
    public new class Request
    {
        [JsonPropertyName("api_url")] public string? ApiUrl { get; set; }
    }

    private readonly AzureAdOptions _azureAdOptions;
    private TokenService _tokenService;
    
    public DelegatedController(TokenService tokenService, IOptions<AzureAdOptions> azureAdOptions)
    {
        _tokenService = tokenService;
        _azureAdOptions = azureAdOptions.Value;
    }
    
    [HttpPost]
    public async Task<string> Post([FromBody] Request body)
    {
        //using var client = new HttpClient();
        //var token = await _tokenService.GetAccessTokenAsync(HttpContext.User, _azureAdOptions.GraphScopes);
        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //return await client.GetStringAsync(body.ApiUrl);
        return "token";
    }
}
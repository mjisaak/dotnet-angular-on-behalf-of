using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class OnBehalfOfController : ControllerBase
    {
        public new class Request
        {
            [JsonPropertyName("api_url")] public string? ApiUrl { get; set; }
        }

        private readonly TokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public OnBehalfOfController(IOptions<AzureAdOptions> options, TokenService tokenService, IHttpClientFactory httpClientFactory)
        {
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] Request request)
        {
            using var client = _httpClientFactory.CreateClient();
            var authenticationResult = await _tokenService.GetAccessTokenAsync(HttpContext, new[] { "" }/* Scopes can be empty for some reason */);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, authenticationResult.AccessToken);

            return Ok(await client.GetStringAsync(request.ApiUrl!));
        }
    }
}

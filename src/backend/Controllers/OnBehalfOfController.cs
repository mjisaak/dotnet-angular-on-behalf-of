using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using backend.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using Microsoft.Identity.Web.Resource;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class OnBehalfOfController : ControllerBase
    {
        public class Request
        {
            [JsonPropertyName("api_url")] public string ApiUrl { get; set; }
        }

        private readonly AzureAdOptions _azureAdOptions;
        private readonly TokenService _tokenService;

        public OnBehalfOfController(IOptions<AzureAdOptions> options, TokenService tokenService)
        {
            _azureAdOptions = options.Value;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] Request request)
        {
            var client = new HttpClient();
            var authenticationResult = await _tokenService.GetAccessTokenAsync(HttpContext, new []{ "https://graph.microsoft.com/User.Read.All" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

            return Ok(await client.GetStringAsync(request.ApiUrl));
        }
    }
}

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OnBehalfOfController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public OnBehalfOfController(TokenService tokenService, IHttpClientFactory httpClientFactory)
        {
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var client = _httpClientFactory.CreateClient();

            var authenticationResult = await _tokenService.GetAccessTokenAsync(HttpContext, new List<string>() { "https://graph.microsoft.com/User.ReadWrite.All" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.Bearer, authenticationResult.AccessToken);

            return Ok(await client.GetStringAsync("https://graph.microsoft.com/v1.0/me"));
        }
    }
}

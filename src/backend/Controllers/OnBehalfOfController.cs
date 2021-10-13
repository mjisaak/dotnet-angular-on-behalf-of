using backend.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using backend.Services;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OnBehalfOfController : ControllerBase
    {
        private readonly AzureAdOptions _azureAdOptions;
        private readonly TokenService _tokenService;

        public OnBehalfOfController(IOptions<AzureAdOptions> options, TokenService tokenService)
        {
            _azureAdOptions = options.Value;
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Get()
        {
            var token = await _tokenService.GetAccessTokenAsync(HttpContext.User, new[] { "https://graph.microsoft.com/User.Read.All" });
            return Ok(token);
        }
    }
}

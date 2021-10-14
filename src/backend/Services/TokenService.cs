using System.Security.Claims;
using backend.Options;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace backend.Services
{
    public class TokenService
    {
        private readonly AzureAdOptions _azureAdOptions;

        public TokenService(IOptions<AzureAdOptions> options)
        {
            _azureAdOptions = options.Value;
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync(HttpContext context, IEnumerable<string> scopes)
        {
            var userAccessToken = context.Request.Headers.Authorization
                .First()
                .Replace("Bearer", "");

            var app = BuildApp(context.User);
            var userAssertion = new UserAssertion(userAccessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");

            var result = await app.AcquireTokenOnBehalfOf(scopes, userAssertion)
                .ExecuteAsync();
            return result;
        }

        private IConfidentialClientApplication BuildApp(ClaimsPrincipal principal)
        {
            var app = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId)
                .WithClientSecret(_azureAdOptions.ClientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, Guid.Parse(_azureAdOptions.TenantId))
                .Build();


            return app;
        }
    }
}
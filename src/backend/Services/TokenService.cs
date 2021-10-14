using Ardalis.GuardClauses;
using Microsoft.Identity.Client;

namespace backend.Services
{
    public class TokenService
    {
        private readonly IConfidentialClientApplication _application;

        public TokenService(IConfidentialClientApplication application)
        {
            _application = application;
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync(HttpContext context, IEnumerable<string> scopes)
        {
            var userAccessToken = context.Request.Headers.Authorization
                .FirstOrDefault()
                ?.Replace("Bearer", "");
            Guard.Against.Default(userAccessToken, "userAccessToken");

            var userAssertion = new UserAssertion(userAccessToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            var result = await _application.AcquireTokenOnBehalfOf(scopes, userAssertion)
                .ExecuteAsync();

            return result;
        }
    }
}
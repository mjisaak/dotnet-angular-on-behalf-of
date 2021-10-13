using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.Extensions;
using backend.Options;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace backend.Services
{
    public class TokenService
    {
        private readonly ILogger<TokenService> _logger;

        private readonly AzureAdOptions _azureAdOptions;

        public TokenService(ILogger<TokenService> logger, IOptions<AzureAdOptions> options)
        {
            _logger = logger;
            _azureAdOptions = options.Value;
        }

        public async Task<AuthenticationResult> GetAccessTokenByJwtTokenAsync(ClaimsPrincipal principal, JwtSecurityToken jwtToken, IEnumerable<string> scopes)
        {
            var app = BuildApp(principal);
            var userAssertion = new UserAssertion(jwtToken.RawData, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            var result = await app.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<string> GetAccessTokenAsync(ClaimsPrincipal principal, IEnumerable<string> scopes)
        {
            var app = BuildApp(principal);
            var account = await app.GetAccountAsync(principal.GetMsalAccountId());

            // guest??
            if (null == account)
            {
                var accounts = await app.GetAccountsAsync();
                account = accounts.FirstOrDefault(a => a.Username == principal.GetLoginHint());
            }

            var token = await app.AcquireTokenSilent(scopes, account).ExecuteAsync().ConfigureAwait(false);
            return token.AccessToken;
        }

        private IConfidentialClientApplication BuildApp(ClaimsPrincipal principal)
        {
            var app = ConfidentialClientApplicationBuilder.Create(_azureAdOptions.ClientId)
                .WithDebugLoggingCallback()
                .WithClientSecret(_azureAdOptions.ClientSecret)
                // we only allow users from our tenant
                .WithAuthority(AzureCloudInstance.AzurePublic, Guid.Parse(_azureAdOptions.TenantId))
                // reply url
                .Build();


            return app;
        }
    }
}
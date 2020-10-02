using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Web.Services.Abstracts;

namespace Web.Services
{
    /// <summary>
    /// HttpClient for fetching user details from Identity/Auth server using oidc protocol
    /// </summary>
    class OidcUserInfoService : IOidcUserInfoService
    {
        readonly HttpClient HttpClient;
        readonly IConfiguration Configuration;

        public OidcUserInfoService(HttpClient httpClient, IConfiguration configuration)
        {
            HttpClient = httpClient;
            Configuration = configuration;
        }

        public async Task<IEnumerable<Claim>> GetClaims(string accessToken, CancellationToken cancellationToken = default)
        {
            var discoveryResponse = await HttpClient.GetDiscoveryDocumentAsync(Configuration.GetValue<string>("AppOidcAuthority"), cancellationToken);
            if (discoveryResponse.IsError) { throw new Exception(discoveryResponse.Error); }

            var userInfoRequest = new UserInfoRequest 
            { 
                Address = discoveryResponse.UserInfoEndpoint,
                Token = accessToken
            };
            var userInfoResponse = await HttpClient.GetUserInfoAsync(userInfoRequest, cancellationToken);
            if (userInfoResponse.IsError) { throw new Exception(userInfoResponse.Error); }

            return userInfoResponse.Claims;
        }

        public async Task<string> GetUserName(string accessToken, CancellationToken cancellationToken = default)
        {
            string userName = "";

            var claims = await GetClaims(accessToken, cancellationToken);

            var givenNameClaim = claims.First(c => c.Type == JwtClaimTypes.GivenName);
            if (givenNameClaim != null) { userName = givenNameClaim.Value; }

            var familyNameClaim = claims.First(c => c.Type == JwtClaimTypes.FamilyName);
            if (familyNameClaim != null) { userName = $"{familyNameClaim.Value} {userName}"; }

            if (userName == null)
            {
                var fallBackUserNameClaim = claims.First(c => c.Type == JwtClaimTypes.MiddleName ||
                                            c.Type == JwtClaimTypes.Name ||
                                            c.Type == JwtClaimTypes.NickName ||
                                            c.Type == JwtClaimTypes.PreferredUserName);
               userName = fallBackUserNameClaim.Value;
            }

            return userName;
        }
    }
}

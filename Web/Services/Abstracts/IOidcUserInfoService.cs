using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Services.Abstracts
{
    /// <summary>
    /// User info service interface
    /// </summary>
    public interface IOidcUserInfoService
    {
        /// <summary>
        /// Returns a list of the currently authenticated user's claims from the idp
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>claims</returns>
        Task<IEnumerable<Claim>> GetClaims(string accessToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a string that can be used as the authenticated user's name
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>claims</returns>
        Task<string> GetUserName(string accessToken, CancellationToken cancellationToken = default);
    }
}

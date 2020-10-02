using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Web.Auth
{
    /// <summary>
    /// Custom policy provider
    /// </summary>
    class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        readonly DefaultAuthorizationPolicyProvider FallbackPolicyProvider;

        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("Create") ||
                policyName.StartsWith("Delete") ||
                policyName.StartsWith("Edit") ||
                policyName.StartsWith("View"))
            {
                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.AddRequirements(new PermissionRequirement(policyName));

                return Task.FromResult(policyBuilder.Build());
            } else if (policyName.StartsWith("delete.") ||
                policyName.StartsWith("read.") ||
                policyName.StartsWith("worker.") ||
                policyName.StartsWith("write."))
            {
                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.AddRequirements(new ScopeRequirement(policyName));

                return Task.FromResult(policyBuilder.Build());
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}

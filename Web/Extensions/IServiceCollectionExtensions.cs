using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Web.Auth;
using Web.Services;
using Web.Services.Abstracts;
using Web.Utils;

namespace Web.Extensions
{
    /// <summary>
    /// IServiceCollectionExtensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// And and configure cookie settings for the app
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCookieOptions(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            return services;
        }

        /// <summary>
        /// Add and configure authentication services for the app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddScoped<CustomCookieAuthenticationEvents>();
            services.AddScoped<CustomJwtBearerEvents>();
            services.AddScoped<CustomOpenIdConnectEvents>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = configuration.GetValue<string>("AppJwtBearerAuthority");
                options.EventsType = typeof(CustomJwtBearerEvents);
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    // If the access token doesn't contain a sub claim, specify the claim to be used for NameCliamType so that ASP.NET will use it for HttpContext.User.Identity.Name
                    // NameClaimType = "name",
                    RoleClaimType = "role",
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = configuration.GetValue<string>("AppAuthCookieName");
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
                //options.SlidingExpiration = true;
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = configuration.GetValue<string>("AppOidcAuthority");
                options.ClientId = configuration.GetValue<string>("AppOidcClientId");
                options.ClientSecret = configuration.GetValue<string>("AppOidcClientSecret");
                options.EventsType = typeof(CustomOpenIdConnectEvents);
                
                // Don't immediately fetch user claims from user-info endpoint upon retriveing id_token.
                // This is good because user claims change a lot and so storing them locally will lead to working with stale data
                options.GetClaimsFromUserInfoEndpoint = false;
                
                options.ResponseType = "code";
                options.SaveTokens = true;

                // IdenityResources scopes
                options.Scope.Add("offline_access");
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add(Constants.CustomClaimTypes.Permission);

                // TodoItem API resource scopes
                options.Scope.Add(Constants.AuthorizationPolicyNames.DeleteTodoItemScope);
                options.Scope.Add(Constants.AuthorizationPolicyNames.ReadTodoItemScope);
                options.Scope.Add(Constants.AuthorizationPolicyNames.WriteTodoItemScope);

                // When GetCliamsFromUserInfoEndPoint is true, the map actions below specify claims from the user-info endpoint json response 
                // that should be added to the the authenticated user's claims
                //options.ClaimActions.MapJsonKey("address", "address");
                //options.ClaimActions.MapJsonKey("email", "email");
                //options.ClaimActions.MapJsonKey("permission", "permission");
                //options.ClaimActions.MapJsonKey("website", "website");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // If the id token doesn't contain a sub claim, specify the claim to be used for NameCliamType so that ASP.NET will use it for HttpContext.User.Identity.Name
                    //NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

            return services;
        }

        /// <summary>
        /// Add and configure authorization services for the app
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Configure authentication to be required thoroughout except for pages, controllers or actions with [AllowAnonymous] attribute
                //options.FallbackPolicy = new AuthorizationPolicyBuilder()
                //.RequireAuthenticatedUser()
                //.Build();
            });

            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
                .AddScoped<IAuthorizationHandler, ScopeAuthorizationHandler>()
               .AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

            return services;
        }

        /// <summary>
        /// Add and configure localization services for the app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cultureCookieName"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomLocalization(this IServiceCollection services, string cultureCookieName)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources")
                .Configure<RequestLocalizationOptions>(options =>
                {
                    var cultures = new[]           
                    {
                        new CultureInfo("en"),                             
                    };
                    options.DefaultRequestCulture = new RequestCulture("en");
                    options.SupportedCultures = cultures;
                    options.SupportedUICultures = cultures;
                    options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First().CookieName = cultureCookieName;

                    options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(context =>
                    {
                        var result = new ProviderCultureResult("en");
                        return Task.FromResult(result);
                    }));
                });

            return services;
        }

        /// <summary>
        /// Add and configure http clients for the app
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            // Polly policy with exponential back off and some jitter
            Random Jitterer = new Random();
            var retryWithJitterPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(Jitterer.Next(0, 100)));

            var registry = services.AddPolicyRegistry();
            registry.Add(Constants.PollyPolicyNames.RetryWithJitter, retryWithJitterPolicy);

            // Add access token management services that fetch, store (in cookies for currently authenticated user, in memory for client), and automatically refresh access and refresh tokens 
            services.AddAccessTokenManagement(options =>
            {
            }).ConfigureBackchannelHttpClient()
            .AddPolicyHandlerFromRegistry(Constants.PollyPolicyNames.RetryWithJitter);

            // Registers an http client that uses the user access token managed by token management services
            // The http client can be retrieved via HttpClientFactory.Create(nameOfClient)
            services.AddUserAccessTokenClient(Constants.HttpClientNames.ClientWithUserAccessToken, httpClient =>
            {
            }).AddPolicyHandlerFromRegistry(Constants.PollyPolicyNames.RetryWithJitter);

            // Registers an http client that uses the client access token managed by token management services
            services.AddClientAccessTokenClient(Constants.HttpClientNames.ClientWithClientAccessToken, configureClient: httpClient =>
            {
            }).AddPolicyHandlerFromRegistry(Constants.PollyPolicyNames.RetryWithJitter);

            // Register typed HttpClients that also use the user access token managed by token management services
            services.AddHttpClient<IOidcUserInfoService, OidcUserInfoService>()
                .AddUserAccessTokenHandler()
                .AddPolicyHandlerFromRegistry(Constants.PollyPolicyNames.RetryWithJitter);

            return services;
        }

        /// <summary>
        /// Add and configure swagger services for the app
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(string.Format(@"{0}\TodoAPI.xml", AppDomain.CurrentDomain.BaseDirectory));
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Todo API",
                });

                options.OperationFilter<OpenApi.CheckAuthorize>();
                options.OperationFilter<OpenApi.CheckAntiforgery>();
                options.OperationFilter<OpenApi.RemoveVersionParameterFromPath>();
                options.DocumentFilter<OpenApi.ReplaceVersionParameterWithVersionValueInPath>();

                options.AddSecurityDefinition(
                    Constants.SwaggerSecurityDefinitionNames.OAuth2AuthCodeNative,
                    OpenApi.SecuritySchemes.OAuth2AuthCodeSchemeNative(configuration));

                options.AddSecurityDefinition(
                   Constants.SwaggerSecurityDefinitionNames.OAuth2AuthCodeAdminNative,
                   OpenApi.SecuritySchemes.OAuth2AuthCodeSchemeAdminNative(configuration));

                options.AddSecurityDefinition(
                    Constants.SwaggerSecurityDefinitionNames.OAuth2ClientCredentials,
                    OpenApi.SecuritySchemes.OAuth2ClientCredentialsScheme(configuration));
            });

            return services;
        }
    }
}

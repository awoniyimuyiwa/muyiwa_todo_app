using Application;
using FluentValidation.AspNetCore;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using Web.Extensions;
using Web.ViewModels.Transient.v1;

namespace Web
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(Configuration.GetConnectionString("DefaultConnection"))
                .AddCookieOptions()
                .AddCustomAuthentication(Configuration)
                .AddCustomAuthorization()
                .AddCustomLocalization(Configuration.GetValue<string>("AppUserCultureCookieName"))
                .AddHttpClients();

            #region Api Versioning
            // Add API Versioning to the Project
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
                // Support all three methods of specifying api version (url, query string and request header)
                config.ApiVersionReader = Microsoft.AspNetCore.Mvc.Versioning.ApiVersionReader.Combine(new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("x-api-version"), new Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader("api-version"));
            });
            #endregion

            // All fluent validation classes that have direct or indirect dependency on services with scoped lifetime (e.g dbContext, application services, repositories e.t.c) are kept in ViewModels.Transient namespace and registered with transient lifetime
            // while other fluent validation classes that don't have such dependency are kept in ViewModels.Singleton namespace and registered with singleton lifetime to boost performance.
            services.AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<TodoItemViewModelValidator>()
               //.RegisterValidatorsFromAssemblyContaining<ViewModels.Singleton.SampleFluentValidator>(lifetime: ServiceLifetime.Singleton)
               .ValidatorOptions.CascadeMode = FluentValidation.CascadeMode.Stop);

            // Add anti forgery services and support for the way javascript http clients like that of Angular and Axios send csrf token.
            // A cookie with the name XSRF-TOKEN cookie will be stored and sent in the response of /api/identity endpoint which SPAs should call first thing after launch.
            // Javascript http clients like that of Angular and Axios automatically retrieve the value of XSRF-TOKEN cookie and use it to set the value of X-XSRF-TOKEN header in requests
            services.AddAntiforgery(options => {
                options.Cookie.Name = "CSRF-TOKEN";
                options.HeaderName = "X-XSRF-TOKEN";
            });

            // Enable access to HttpContext in custom components that need it
            services.AddHttpContextAccessor();

            // Add and configure swagger services for the app
            services.AddCustomSwaggerGen(Configuration);

            #region Proxy configuration
            // X-Forwarded headers from proxy servers that should be processed when app is behind a proxy server that is not IIS
            if (string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_FORWARDEDHEADERS_ENABLED"),
                "true", StringComparison.OrdinalIgnoreCase))
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                        ForwardedHeaders.XForwardedProto;
                    // Only loopback proxies are allowed by default.
                    // Clear that restriction because forwarders are enabled by explicit 
                    // configuration.
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });
            }
            #endregion
        }

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders()
               .UseHttpsRedirection()
               .UseCookiePolicy()
               .UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseInfrastructureDatabaseErrorPage();
            }

            app.UseCustomExceptionHandler()
               .UseRouting()
               .UseStaticFiles()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });

            #region Configure Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API");

                options.OAuthAppName("Todo API Swagger UI");
                options.OAuthClientId(Configuration.GetValue<string>("AppOidcClientId"));
                options.OAuthClientSecret(Configuration.GetValue<string>("AppOidcClientSecret"));
                options.OAuthUsePkce();
            });
            #endregion
        }
    }
}

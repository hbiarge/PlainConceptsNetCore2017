using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.Infrastructure.Authorization;
using Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Authorization
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add Authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Sales, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("department", "sales");
                });
                options.AddPolicy(Policies.Over18Years, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new MinimumAgeRequirement(18));
                });
            });

            // register resource authorization handlers
            services.AddSingleton<IAuthorizationHandler, ProductAuthorizationHandler>();

            // register services
            services.AddSingleton<IProductsStore, InMemoryProductsStore>();
            services.AddSingleton<IDiscountPermissionService, DefaultDiscountPermissionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticChallenge = true
            });

            app.UseClaimsTransformation(context =>
            {
                var identity = context.Principal.Identities.First();
                identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, DateTime.Now.AddYears(-20).ToString(CultureInfo.InvariantCulture)));
                identity.AddClaim(new Claim("status", "junior"));
                return Task.FromResult(context.Principal);
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

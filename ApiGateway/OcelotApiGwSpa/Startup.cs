using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace XJeunot.PhysicalStoreApps.ApiGateway.OcelotApiGwSpa
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            /* Add Service Ocelot. */
            services.AddOcelot(Configuration);

            /* Add Cors Policy Development*/
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicyDevelopment",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });

            // Add Angular's default header name for sending the XSRF token.
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                /* Activate Cors Policy Development*/
                app.UseCors("CorsPolicyDevelopment");
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            /*
             * Middleware Overload for Add Angular Security in Ocelot Proxy API.
             */
            // TODO: Save Token in Cluster : https://docs.microsoft.com/fr-fr/aspnet/core/security/anti-request-forgery?view=aspnetcore-2.2#authentication-fundamentals.
            app.Use(next => context =>
            {
                // Intercept Only Change Http Methods.
                if (! string.Equals(HttpMethods.Get, context.Request.Method, StringComparison.OrdinalIgnoreCase))
                {
                    // Setup Cookie.
                    if (context.Request.Path.Value.StartsWith("/api/identity"))
                    {
                        // We can send the request token as a JavaScript-readable cookie, and Angular will use it by default.
                        var tokens = antiforgery.GetAndStoreTokens(context);
                        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                            new CookieOptions() { HttpOnly = false });
                    }
                    //Verify Header.
                    else if (context.Request.Path.Value.StartsWith("/api"))
                    {
                        // This will throw if the token is invalid.
                        antiforgery.ValidateRequestAsync(context).Wait();
                    }
                }
                return next(context);
            });

            /* Use Service Ocelot. */
            app.UseOcelot().Wait();
        }
    }
}

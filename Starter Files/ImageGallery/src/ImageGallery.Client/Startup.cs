using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace ImageGallery.Client
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // This clears the default claim mappings, so we can get the exact keys that we want for the requested claims.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }
 
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();


            // This configures the cookie handler and enables us to use cookie based
            // Once an identity token is validated it is encrypted into a cookie used on subsquent reqs to web app
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>        // Here we are defining support options for the open id connect authentication workflow.
            {
                // This handler creating the auth requests, token and other request and handle identiy token validation.
                options.SignInScheme = "Cookies";
                options.Authority = "https://localhost:44356"; // IDP
                options.ClientId = "imagegalleryclient";
                options.ResponseType = "code id_token"; // This is where we define our flow. The response type the server will send.  this is a hybrid method, where the identity token and authorisation code is returned from auth endpoint.
                // options.CallbackPath = new PathString("..."); the redirect path on the client that the tokens are sent to.
                //options.Scope.Add("openid");
                //options.Scope.Add("profile"); // these two scopes are requested in default.
                options.Scope.Add("address");
                options.SaveTokens = true; // Allows the middleweat to save the tokens recieved from the identity provider.
                options.ClientSecret = "secret";
                options.GetClaimsFromUserInfoEndpoint = true;
                // Claim actions are filters used to stop certain claims being passed through from the json web token to our claims identity, stored in the httpcontext.user
                // The remove statemetn removes the filter for the amr, which means that it CAN be passed from the web token.
                options.ClaimActions.Remove("amr");
                // Delete claim adds a filter for the specified claim which means It CANNOT be passed from the web token.
                options.ClaimActions.DeleteClaim("sud");
                // options.ClaimActions.DeleteClaim("address");

            });
            // Add framework services.
            services.AddMvc();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}

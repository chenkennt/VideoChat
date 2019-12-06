using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.SignalR.VideoChat
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddGitHub(options =>
            {
                options.ClientId = Configuration["GithubClientId"];
                options.ClientSecret = Configuration["GithubClientSecret"];
                options.ClaimActions.MapJsonKey(AuthController.AvatarUrlClaim, "avatar_url");
            }).AddCookie(options =>
            {
                options.LoginPath = "/signin";
                options.LogoutPath = "/signout";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Chat>("/chat");
            });
        }
    }
}

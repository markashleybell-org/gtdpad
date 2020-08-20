using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Nancy.Owin;

namespace gtdpad
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var configPath = env.ContentRootPath + $"/config.{env.EnvironmentName.ToLower()}.json";

            var bootstrapper = new GTDPadBootstrapper(configPath);

            app.UseOwin(x => x.UseNancy(n => n.Bootstrapper = bootstrapper));
        }
    }
}

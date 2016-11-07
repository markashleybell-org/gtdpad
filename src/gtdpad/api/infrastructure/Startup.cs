using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace gtdpad
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(n => n.Bootstrapper = new GTDPadBootstrapper()));
        }
    }
}
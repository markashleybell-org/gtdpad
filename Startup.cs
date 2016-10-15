using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy.Configuration;
using Nancy.Diagnostics;
using Nancy;

namespace gtdpad
{
    public class GTDPadBootstrapper : DefaultNancyBootstrapper
    {
        public override void Configure(INancyEnvironment environment)
        {
            environment.Diagnostics(
                enabled: true,
                password: "test123"
            );

            environment.Tracing(
                enabled: false, 
                displayErrorTraces: true
            );
        }
    }

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(n => n.Bootstrapper = new GTDPadBootstrapper()));
        }
    }
}
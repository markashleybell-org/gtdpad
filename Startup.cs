using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Authentication.Forms;
using Nancy.Diagnostics;
using Nancy;
using Nancy.TinyIoc;

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

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register<IUserMapper, UserDatabase>();
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var formsAuthConfiguration = new FormsAuthenticationConfiguration {
                RedirectUrl = "~/login",
                UserMapper = container.Resolve<IUserMapper>(),
            };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
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
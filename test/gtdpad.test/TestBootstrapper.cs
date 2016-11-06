using System;
using System.Collections.Generic;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Authentication.Forms;
using Nancy.Diagnostics;
using Nancy.Conventions;
using Nancy;
using Nancy.TinyIoc;

namespace gtdpad.test 
{
    public class TestBootstrapper : DefaultNancyBootstrapper
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

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            var staticFolderConventions = new List<Func<NancyContext, string, Response>> {
                StaticContentConventionBuilder.AddDirectory("js", @"static/js"),
                StaticContentConventionBuilder.AddDirectory("css", @"static/css"),
                StaticContentConventionBuilder.AddDirectory("img", @"static/img")
            };

            staticFolderConventions.ForEach(sfc => conventions.StaticContentsConventions.Add(sfc));
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // Don't call base.ConfigureApplicationContainer to avoid auto-registration 
            // of discovered types with the application container
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            
            container.Register<IUserMapper, FakeRepository>();
            container.Register<IRepository, FakeRepository>();
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
}
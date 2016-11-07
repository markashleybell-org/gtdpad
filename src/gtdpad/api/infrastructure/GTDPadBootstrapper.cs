using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Cryptography;
using Nancy.Authentication.Forms;
using Nancy.Diagnostics;
using Nancy.Conventions;
using Nancy;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;

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
            
            // var repository = new Repository(context.)

            container.Register<IUserMapper, Repository>();
            container.Register<IRepository, Repository>();
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var cryptographyConfiguration = new CryptographyConfiguration(
                new AesEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })),
                new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }))
            );

            var formsAuthConfiguration = new FormsAuthenticationConfiguration {
                CryptographyConfiguration = cryptographyConfiguration,
                RedirectUrl = "~/login",
                UserMapper = container.Resolve<IUserMapper>()
            };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}
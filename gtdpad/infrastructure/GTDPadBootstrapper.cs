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
using Newtonsoft.Json.Linq;
using System.IO;

namespace gtdpad
{
    public class GTDPadBootstrapper : DefaultNancyBootstrapper
    {
        private readonly string _configurationFile;

        public GTDPadBootstrapper(string configurationFile) =>
            _configurationFile = configurationFile;

        public override void Configure(INancyEnvironment environment)
        {
            var config = JObject.Parse(File.ReadAllText(_configurationFile));

            foreach(var element in config)
            {
                environment.AddValue<string>(element.Key, element.Value.Value<string>());
            }

            environment.Diagnostics(
                enabled: true,
                password: config["GTDPad.DiagnosticsPassword"].Value<string>()
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
                StaticContentConventionBuilder.AddDirectory("img", @"static/img"),
                StaticContentConventionBuilder.AddDirectory("font", @"static/font"),
                StaticContentConventionBuilder.AddDirectory(".well-known", ".well-known")
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
            
            var repository = new Repository(context.Environment["GTDPad.ConnectionString"].ToString());

            container.Register<IUserMapper, Repository>(repository);
            container.Register<IRepository, Repository>(repository);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            var aesPassphrase = context.Environment["GTDPad.AESPassphrase"].ToString();
            var hmacPassphrase = context.Environment["GTDPad.HMACPassphrase"].ToString();

            var cryptographyConfiguration = new CryptographyConfiguration(
                new AesEncryptionProvider(new PassphraseKeyGenerator(aesPassphrase, new byte[] { 94, 132, 251, 176, 174, 17, 247, 23 })),
                new DefaultHmacProvider(new PassphraseKeyGenerator(hmacPassphrase, new byte[] { 220, 141, 215, 209, 243, 217, 174, 245 }))
            );

            var formsAuthConfiguration = new FormsAuthenticationConfiguration {
                CryptographyConfiguration = cryptographyConfiguration,
                RedirectUrl = context.Environment["GTDPad.LoginRedirect"].ToString(),
                UserMapper = container.Resolve<IUserMapper>(),
                RequiresSSL = !Convert.ToBoolean(context.Environment["GTDPad.DevMode"])
            };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}

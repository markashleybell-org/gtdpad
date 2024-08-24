using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;

namespace gtdpad
{
    public static class Program
    {
        private static readonly string _certPath = Environment.GetEnvironmentVariable("CF_ORIGIN_CERT_PATH");
        private static readonly string _keyPath = Environment.GetEnvironmentVariable("CF_ORIGIN_KEY_PATH");

        private static readonly X509Certificate2 _certificate = X509Certificate2.CreateFromPemFile(_certPath, _keyPath);

        public static void Main(string[] _) =>
            new WebHostBuilder()
                .UseKestrel(o => {
                    o.AllowSynchronousIO = true;

                    o.ListenAnyIP(5001, listenOptions =>
                    {
                        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                        {
                            listenOptions.UseHttps(_certificate);
                        }
                        else
                        {
                            listenOptions.UseHttps();
                        }
                    });
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
}

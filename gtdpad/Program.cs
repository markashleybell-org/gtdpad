using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace gtdpad
{
    public static class Program
    {
        public static void Main(string[] _) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
}

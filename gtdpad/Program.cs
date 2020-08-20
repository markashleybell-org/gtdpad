using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace gtdpad
{
    public static class Program
    {
        public static void Main(string[] _) =>
            new WebHostBuilder()
                .UseKestrel(o => o.AllowSynchronousIO = true)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
}

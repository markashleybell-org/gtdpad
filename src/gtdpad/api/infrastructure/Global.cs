using System.Net;
using System.Net.Http;

namespace gtdpad
{
    public static class Global
    {
        public static HttpClient HttpClient { get; }

        static Global()
        {
            HttpClient  = new HttpClient(new HttpClientHandler {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

            HttpClient.DefaultRequestHeaders.Add(
                "User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36"
            );
        }
    }
}
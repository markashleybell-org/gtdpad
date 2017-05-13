using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nancy.Helpers;
using HtmlAgilityPack;

namespace gtdpad
{
    public static class Global
    {
        // TODO: Is this bad practice? See http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
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

        private static async Task<string> FetchMetadataAsync(string url)
        {
            try
            {
                return await Global.HttpClient.GetStringAsync(url);
            }
            catch
            {
                return null;
            }
        }

        public static Metadata FetchAndParseMetadata(string rqurl)
        {
            var content = FetchMetadataAsync(rqurl).Result;
            
            if(!string.IsNullOrWhiteSpace(content))
            {
                var html = new HtmlDocument();
                html.LoadHtml(content);

                var data = new Metadata {
                    Title = HttpUtility.HtmlDecode(html.DocumentNode.SelectSingleNode("//title")?.InnerText)
                };

                var titleTags = new List<string> { 
                    "//meta[@property='og:title']", 
                    "//meta[@property='twitter:title']" 
                };

                var descriptionTags = new List<string> {
                    "//meta[@name='description']",
                    "//meta[@property='og:description']",
                    "//meta[@property='twitter:description']"
                };

                var imageTags = new List<string> {
                    "//meta[@property='og:image']",
                    "//meta[@property='twitter:image']"
                };

                var urlTags = new List<string> {
                    "//meta[@property='og:url']",
                    "//meta[@property='twitter:url']"
                };

                titleTags.ForEach(xpath => {
                    var title = html.GetText(xpath);
                    if(!string.IsNullOrWhiteSpace(title))
                        data.Title = HttpUtility.HtmlDecode(title);
                });

                // descriptionTags.ForEach(xpath => {
                //     var description = html.GetText(xpath);
                //     if(!string.IsNullOrWhiteSpace(description))
                //         data.Description = description;
                // });

                // imageTags.ForEach(xpath => {
                //     var image = html.GetText(xpath);
                //     if(!string.IsNullOrWhiteSpace(image))
                //         data.Image = image;
                // });

                // urlTags.ForEach(xpath => {
                //     var url = html.GetText(xpath);
                //     if(!string.IsNullOrWhiteSpace(url))
                //         data.Url = url;
                // });

                return data;
            }

            return null;
        }
    }
}
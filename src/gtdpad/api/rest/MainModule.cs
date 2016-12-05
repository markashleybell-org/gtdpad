using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using HtmlAgilityPack;

namespace gtdpad
{
    public class MainModule : NancyModule
    {
        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        private IndexViewModel BuildIndexViewModel(IRepository db, GTDPadIdentity user, Guid? pageID = null)
        {
            var pages = db.ReadPages(user.Identifier);

            if(!pageID.HasValue)
                pageID = pages.First().ID;

            // Build up the initial data structure
            var data = new { 
                contentData = db.ReadPageDeep(pageID.Value),
                sidebarData = new {
                    pages = pages 
                }
            };
            
            return new IndexViewModel {
                LoggedIn = true,
                Username = user.Name,
                InitialData = JsonConvert.SerializeObject(data, _jsonSettings)
            };
        }

        public static async Task<string> FetchMetadata(string url)
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

        private Metadata ExtractMetaData(string rqurl)
        {
            var content = FetchMetadata(rqurl).Result;
            
            if(!string.IsNullOrWhiteSpace(content))
            {
                var html = new HtmlDocument();
                html.LoadHtml(content);

                var data = new Metadata {
                    // Url = rqurl,
                    Title = html.DocumentNode.SelectSingleNode("//title")?.InnerText
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
                        data.Title = title;
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

        public MainModule(IRepository db)
        {
            Get("/", args => {
                this.RequiresAuthentication();
                return View["index.html", BuildIndexViewModel(db, this.GetUser())];
            });

            Get("/{id:guid}", args => {
                this.RequiresAuthentication();
                return View["index.html", BuildIndexViewModel(db, this.GetUser(), args.id)];
            });

            Get("/signup", args => {
                return View["signup.html", new BaseViewModel()];
            });

            Post("/signup", args => {
                var existing = db.GetUserID((string)this.Request.Form.Username);
                if(existing.HasValue)
                    return this.Response.AsRedirect("/login");
                var id = db.CreateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                var page = new Page { UserID = id, Title = "Your First Page" };
                page.SetDefaults<Page>();
                db.CreatePage(page);
                return this.LoginAndRedirect(id, cookieExpiry: DateTime.Now.AddDays(30));
            });

            Get("/login", args => {
                return View["login.html", new BaseViewModel()];
            });

            Post("/login", args => {
                var id = db.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                if(id.HasValue)
                    return this.LoginAndRedirect(id.Value, cookieExpiry: DateTime.Now.AddDays(30));
                return View["login.html", new BaseViewModel()];
            });

            Get("/logout", args => {
                return this.LogoutAndRedirect("~/");
            });

            Get("/metadata", args => {
                return ExtractMetaData(this.Request.Query["url"]);
            });

            Get("/tests", args => {
                return View["tests/tests.html"];
            });
        }
    }
}
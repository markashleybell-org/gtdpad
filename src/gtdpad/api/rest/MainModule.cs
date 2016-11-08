using System.Linq;
using Nancy;
using Nancy.Security;
using Nancy.Authentication.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace gtdpad
{
    public class MainModule : NancyModule
    {
        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        public MainModule(IRepository db)
        {
            Get("/", args => {
                this.RequiresAuthentication();

                // Fetch the initial data for this page
                var pages = db.ReadPages(this.GetUser().Identifier);
                var page = pages.First();

                // Build up the initial data structure
                var data = new { 
                    contentData = db.ReadPageDeep(page.ID),
                    sidebarData = new {
                        pages = pages 
                    }
                };
                
                var model = new IndexViewModel {
                    InitialData = JsonConvert.SerializeObject(data, _jsonSettings)
                };

                return View["index.html", model];
            });

            Get("/{id:guid}", args => {
                this.RequiresAuthentication();
                
                var pages = db.ReadPages(this.GetUser().Identifier);

                // Build up the initial data structure
                var data = new { 
                    contentData = db.ReadPageDeep(args.id),
                    sidebarData = new {
                        pages = pages 
                    }
                };
                
                var model = new IndexViewModel {
                    InitialData = JsonConvert.SerializeObject(data, _jsonSettings)
                };

                return View["index.html", model];
            });

            Get("/signup", args => {
                return View["signup.html"];
            });

            Post("/signup", args => {
                var existing = db.GetUserID((string)this.Request.Form.Username);
                if(existing.HasValue)
                    return this.Response.AsRedirect("/login");
                var id = db.CreateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                db.CreatePage(new Page { UserID = id, Title = "Your First Page" });
                return this.LoginAndRedirect(id, null);
            });

            Get("/login", args => {
                return View["login.html"];
            });

            Post("/login", args => {
                var id = db.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                if(id.HasValue)
                    return this.LoginAndRedirect(id.Value, null);
                return View["login.html"];
            });

            Get("/logout", args => {
                return this.LogoutAndRedirect("~/");
            });

            Get("/tests", args => {
                return View["tests/tests.html"];
            });
        }
    }
}
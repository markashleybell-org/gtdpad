using System;
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

        private IndexViewModel BuildIndexViewModel(IRepository db, Guid userID, Guid? pageID = null)
        {
            var pages = db.ReadPages(this.GetUser().Identifier);

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
                InitialData = JsonConvert.SerializeObject(data, _jsonSettings)
            };
        }

        public MainModule(IRepository db)
        {
            Get("/", args => {
                this.RequiresAuthentication();
                return View["index.html", BuildIndexViewModel(db, this.GetUser().Identifier)];
            });

            Get("/{id:guid}", args => {
                this.RequiresAuthentication();
                return View["index.html", BuildIndexViewModel(db, this.GetUser().Identifier, args.id)];
            });

            Get("/signup", args => {
                return View["signup.html"];
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
                return View["login.html"];
            });

            Post("/login", args => {
                var id = db.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                if(id.HasValue)
                    return this.LoginAndRedirect(id.Value, cookieExpiry: DateTime.Now.AddDays(30));
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
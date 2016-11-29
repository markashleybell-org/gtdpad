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

            Get("/tests", args => {
                return View["tests/tests.html"];
            });
        }
    }
}
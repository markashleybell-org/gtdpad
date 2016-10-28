using System;
using System.Linq;
using Nancy;
// using Nancy.Security;
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
                // this.RequiresAuthentication();

                // Fetch the initial data for this page
                var pages = db.ReadPages(new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9"));

                // TODO: This is obviously pretty inefficient at the moment! We need to return a deep object graph in one hit.
                var page = pages.OrderBy(p => p.DisplayOrder).First();
                var lists = db.ReadLists(page.ID);

                var listModels = lists.Select(l => new { 
                    id = l.ID,
                    name = l.Name,
                    displayOrder = l.DisplayOrder,
                    items = db.ReadItems(l.ID)
                });

                // Build up the initial data structure
                var data = new { 
                    contentData = new { 
                        id = page.ID,
                        name = page.Name,
                        lists = listModels
                    },
                    sidebarData = new {
                        pages = pages 
                    }
                };
                
                var model = new IndexViewModel {
                    InitialData = JsonConvert.SerializeObject(data, _jsonSettings)
                };

                return View["index.html", model];
            });

            Get("/login", args => {
                return View["login.html"];
            });

            Post("/login", args => {
                var id = db.ValidateUser((string)this.Request.Form.Username, (string)this.Request.Form.Password);
                return this.LoginAndRedirect(id.Value, null);
            });

            Get("/logout", args => {
                return this.LogoutAndRedirect("~/");
            });
        }
    }
}
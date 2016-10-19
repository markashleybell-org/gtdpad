using Nancy;
// using Nancy.Security;
using Nancy.ModelBinding;
using Nancy.Authentication.Forms;
using System;

namespace gtdpad
{
    public class Main : NancyModule
    {
        public Main()
        {
            var db = new Repository();

            Get("/", args => {
                // this.RequiresAuthentication();
                return View["index.html"];
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

            Post("/pages", args => {
                return db.CreatePage(this.Bind<Page>());
            });

            Get("/pages/{id:guid}", args => {
                return db.ReadPage(args.id);
            });

            Put("/pages/{id:guid}", args => {
                return db.UpdatePage(this.Bind<Page>());
            });

            Delete("/pages/{id:guid}", args => {
                return db.DeletePage(args.id);
            });

            Get("/pages", args => {
                return db.ListPages(new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9"));
            });
        }
    }
}
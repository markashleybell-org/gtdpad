using Nancy;
// using Nancy.Security;
using Nancy.ModelBinding;
// using Nancy.Authentication.Forms;
using System;

namespace gtdpad
{
    public class PagesModule : NancyModule
    {
        public PagesModule(IRepository db) : base("/pages")
        {
            Post("/", args => {
                return db.CreatePage(this.Bind<Page>().SetDefaults<Page>());
            });

            Get("/{id:guid}", args => {
                return db.ReadPage(args.id);
            });

            Put("/{id:guid}", args => {
                return db.UpdatePage(this.Bind<Page>().SetDefaults<Page>());
            });

            Delete("/{id:guid}", args => {
                return db.DeletePage(args.id);
            });

            Get("/", args => {
                return db.ReadPages(new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9"));
            });
        }
    }
}
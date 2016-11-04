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
                var page = this.Bind<Page>().SetDefaults<Page>();
                page.UserID = new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9");
                return db.CreatePage(page);
            });

            Get("/{id:guid}", args => {
                if(this.Request.Query["deep"] != null)
                    return db.ReadPageDeep(args.id);     
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

            Get("/default", args => {
                var defaultPageID = db.ReadDefaultPageID(new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9"));
                if(this.Request.Query["deep"] != null)
                    return db.ReadPageDeep(defaultPageID);     
                return db.ReadPage(defaultPageID);
            });

            Put("/updateorder", args => {
                var ordering = this.Bind<Ordering>();
                ordering.ID = new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9");
                db.UpdatePageDisplayOrder(ordering);
                return true;
            });
        }
    }
}
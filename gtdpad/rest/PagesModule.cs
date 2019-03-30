using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;

namespace gtdpad
{
    public class PagesModule : NancyModule
    {
        public PagesModule(IRepository db) : base("/pages")
        {
            this.RequiresAuthentication();

            Get("/", args => {
                return db.ReadPages(this.GetUser().Identifier);
            });

            Post("/", args => {
                var page = this.Bind<Page>().SetDefaults<Page>();
                page.UserID = this.GetUser().Identifier;
                return db.CreatePage(page);
            });

            Get("/{id:guid}", args => {
                if(Request.Query["deep"] != null)
                    return db.ReadPageDeep(args.id);     
                return db.ReadPage(args.id);
            });

            Put("/{id:guid}", args => {
                return db.UpdatePage(this.Bind<Page>().SetDefaults<Page>());
            });

            Delete("/{id:guid}", args => {
                return db.DeletePage(args.id);
            });

            Get("/default", args => {
                var defaultPageID = db.ReadDefaultPageID(this.GetUser().Identifier);
                if(Request.Query["deep"] != null)
                    return db.ReadPageDeep(defaultPageID);     
                return db.ReadPage(defaultPageID);
            });

            Put("/updateorder", args => {
                var ordering = this.Bind<Ordering>();
                ordering.ID = this.GetUser().Identifier;
                db.UpdatePageDisplayOrder(ordering);
                return true;
            });
        }
    }
}

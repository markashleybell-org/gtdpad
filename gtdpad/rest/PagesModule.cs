using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace gtdpad
{
    public class PagesModule : NancyModule
    {
        public PagesModule(IRepository db)
            : base("/pages")
        {
            this.RequiresAuthentication();

            Get("/", _ => db.ReadPages(this.GetUser().Identifier));

            Post("/", _ => {
                var page = this.Bind<Page>().SetDefaults<Page>();
                page.UserID = this.GetUser().Identifier;
                return db.CreatePage(page);
            });

            Get("/{id:guid}", args => {
                if (Request.Query["deep"] != null)
                {
                    return db.ReadPageDeep(args.id);
                }

                return db.ReadPage(args.id);
            });

            Put("/{id:guid}", _ => db.UpdatePage(this.Bind<Page>().SetDefaults<Page>()));

            Delete("/{id:guid}", args => db.DeletePage(args.id));

            Get("/default", _ => {
                var defaultPageID = db.ReadDefaultPageID(this.GetUser().Identifier);
                if (Request.Query["deep"] != null)
                {
                    return db.ReadPageDeep(defaultPageID);
                }

                return db.ReadPage(defaultPageID);
            });

            Put("/updateorder", _ => {
                var ordering = this.Bind<Ordering>();
                ordering.ID = this.GetUser().Identifier;
                db.UpdatePageDisplayOrder(ordering);
                return true;
            });
        }
    }
}

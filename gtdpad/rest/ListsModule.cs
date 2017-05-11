using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;

namespace gtdpad
{
    public class ListsModule : NancyModule
    {
        public ListsModule(IRepository db) : base("/pages/{pageid:guid}/lists")
        {
            this.RequiresAuthentication();

            Post("/", args => {
                return db.CreateList(this.Bind<List>().SetDefaults<List>());
            });

            Get("/{id:guid}", args => {
                return db.ReadList(args.id);
            });

            Put("/{id:guid}", args => {
                return db.UpdateList(this.Bind<List>().SetDefaults<List>());
            });

            Delete("/{id:guid}", args => {
                return db.DeleteList(args.id);
            });

            Put("/updateorder", args => {
                var ordering = this.Bind<Ordering>();
                db.UpdateListDisplayOrder(ordering);
                return true;
            });
        }
    }
}
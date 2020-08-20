using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace gtdpad
{
    public class ListsModule : NancyModule
    {
        public ListsModule(IRepository db)
            : base("/pages/{pageid:guid}/lists")
        {
            this.RequiresAuthentication();

            Post("/", _ => db.CreateList(this.Bind<List>().SetDefaults<List>()));

            Get("/{id:guid}", args => db.ReadList(args.id));

            Put("/{id:guid}", _ => db.UpdateList(this.Bind<List>().SetDefaults<List>()));

            Delete("/{id:guid}", args => db.DeleteList(args.id));

            Put("/move", _ => {
                var move = this.Bind<ListMove>();
                var list = db.ReadList(move.ListID);
                list.PageID = move.NewPageID;
                db.UpdateList(list);
                db.MoveListToTopOfPage(list.ID);
                return true;
            });

            Put("/updateorder", _ => {
                var ordering = this.Bind<Ordering>();
                db.UpdateListDisplayOrder(ordering);
                return true;
            });
        }
    }
}

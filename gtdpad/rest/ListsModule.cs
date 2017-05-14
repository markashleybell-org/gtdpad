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

            Put("/move", args => {
                var move = this.Bind<ListMove>();
                var list = db.ReadList(move.ListID);
                list.PageID = move.NewPageID;
                db.UpdateList(list);
                db.MoveListToTopOfPage(list.ID);
                return true;
            });

            Put("/updateorder", args => {
                var ordering = this.Bind<Ordering>();
                db.UpdateListDisplayOrder(ordering);
                return true;
            });
        }
    }
}
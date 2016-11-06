using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;

namespace gtdpad
{
    public class ItemsModule : NancyModule
    {
        public ItemsModule(IRepository db) : base("/pages/{pageid:guid}/lists/{listid:guid}/items")
        {
            this.RequiresAuthentication();

            Post("/", args => {
                return db.CreateItem(this.Bind<Item>().SetDefaults<Item>());
            });

            Get("/{id:guid}", args => {
                return db.ReadItem(args.id);
            });

            Put("/{id:guid}", args => {
                return db.UpdateItem(this.Bind<Item>().SetDefaults<Item>());
            });

            Delete("/{id:guid}", args => {
                return db.DeleteItem(args.id);
            });

            Put("/updateorder", args => {
                var ordering = this.Bind<Ordering>();
                db.UpdateItemDisplayOrder(ordering);
                return true;
            });
        }
    }
}
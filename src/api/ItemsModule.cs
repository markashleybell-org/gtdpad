using Nancy;
// using Nancy.Security;
using Nancy.ModelBinding;
// using Nancy.Authentication.Forms;

namespace gtdpad
{
    public class ItemsModule : NancyModule
    {
        public ItemsModule(IRepository db) : base("/pages/{pageid:guid}/lists/{listid:guid}/items")
        {
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

            Get("/", args => {
                return db.ReadItems(args.listid);
            });
        }
    }
}
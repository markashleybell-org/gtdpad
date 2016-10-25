using Nancy;
// using Nancy.Security;
using Nancy.ModelBinding;
// using Nancy.Authentication.Forms;
using System;

namespace gtdpad
{
    public class ItemsModule : NancyModule
    {
        public ItemsModule(IRepository db) : base("/items")
        {
            Post("/", args => {
                return db.CreateItem(this.Bind<Item>());
            });

            Get("/{id:guid}", args => {
                return db.ReadItem(args.id);
            });

            Put("/{id:guid}", args => {
                return db.UpdateItem(this.Bind<Item>());
            });

            Delete("/{id:guid}", args => {
                return db.DeleteItem(args.id);
            });

            Get("", args => {
                return db.ReadItems(new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9"));
            });
        }
    }
}
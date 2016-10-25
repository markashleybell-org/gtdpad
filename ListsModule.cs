using Nancy;
// using Nancy.Security;
using Nancy.ModelBinding;
// using Nancy.Authentication.Forms;
using System;

namespace gtdpad
{
    public class ListsModule : NancyModule
    {
        public ListsModule(IRepository db) : base("/lists")
        {
            Post("/", args => {
                return db.CreateList(this.Bind<List>());
            });

            Get("/{id:guid}", args => {
                return db.ReadList(args.id);
            });

            Put("/{id:guid}", args => {
                return db.UpdateList(this.Bind<List>());
            });

            Delete("/{id:guid}", args => {
                return db.DeleteList(args.id);
            });

            Get("", args => {
                return db.ReadLists(new Guid("47D2911F-C127-40C8-A39A-FB13634D2AE9"));
            });
        }
    }
}
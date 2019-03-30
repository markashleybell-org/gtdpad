using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System;

namespace gtdpad
{
    public class ItemsModule : NancyModule
    {
        private bool IsUrl(string input) =>
            input.IndexOf("http://") == 0 || input.IndexOf("https://") == 0;

        private string[] Words(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return new string[0];

            return input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private Item TryPopulateMetadata(ItemsModule module)
        {
            var item = this.Bind<Item>().SetDefaults<Item>();
            var words = Words(item.Body);
            
            item.Title = null;

            if(words.Length > 0 && IsUrl(words[0]))
            {
                var metadata = Global.FetchAndParseMetadata(words[0]);
                item.Title = metadata != null ? metadata.Title : item.Body;
            }

            return item;
        }

        public ItemsModule(IRepository db) : base("/pages/{pageid:guid}/lists/{listid:guid}/items")
        {
            this.RequiresAuthentication();

            Post("/", args => {
                return db.CreateItem(TryPopulateMetadata(this));
            });

            Get("/{id:guid}", args => {
                return db.ReadItem(args.id);
            });

            Put("/{id:guid}", args => {
                return db.UpdateItem(TryPopulateMetadata(this));
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

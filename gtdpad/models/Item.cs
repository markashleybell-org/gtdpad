using System;

namespace gtdpad
{
    public class Item : IModel
    {
        public Guid ID { get; set; }

        public Guid ListID { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}

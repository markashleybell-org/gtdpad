using System;
using System.Collections.Generic;

namespace gtdpad
{
    public class List : IModel
    {
        public Guid ID { get; set; }

        public Guid PageID { get; set; }

        public string Title { get; set; }

        public IEnumerable<Item> Items { get; set; }
    }
}

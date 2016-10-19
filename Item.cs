using System;

namespace gtdpad
{
    public class Item
    {
        public Guid ID { get; set; }
        public Guid ListID { get; set; }
        public string Text { get; set; }
        public int DisplayOrder { get; set; }
    }
}
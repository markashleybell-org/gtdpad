using System;

namespace gtdpad
{
    public class Page
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
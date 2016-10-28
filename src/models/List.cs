using System;

namespace gtdpad
{
    public class List
    {
        public Guid ID { get; set; }
        public Guid PageID { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
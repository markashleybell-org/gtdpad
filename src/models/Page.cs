using System;

namespace gtdpad
{
    public class Page : IModel
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public string Name { get; set; }
    }
}
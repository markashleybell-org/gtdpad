using System;
using System.Collections.Generic;

namespace gtdpad
{
    public class Page : IModel
    {
        public Guid ID { get; set; }

        public Guid UserID { get; set; }

        public string Title { get; set; }

        public IEnumerable<List> Lists { get; set; }
    }
}

using System;

namespace gtdpad
{
    public interface IModel 
    {
        Guid ID { get; set; }
        int DisplayOrder { get; set; }
    }
}
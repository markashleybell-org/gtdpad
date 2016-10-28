using System;

namespace gtdpad
{
    public static class ModelExtensions
    {
        public static T SetDefaults<T>(this IModel model)
        {
            if(model.ID == Guid.Empty)
                model.ID = Guid.NewGuid();

            if(model.DisplayOrder == 0) 
                model.DisplayOrder = int.MaxValue;

            return (T)model;
        }
    }
}
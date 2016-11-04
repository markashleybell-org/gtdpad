using System;

namespace gtdpad
{
    public static class ModelExtensions
    {
        public static T SetDefaults<T>(this IModel model)
        {
            if(model.ID == Guid.Empty)
                model.ID = Guid.NewGuid();

            return (T)model;
        }
    }
}
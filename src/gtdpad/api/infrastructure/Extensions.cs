using System;
using Nancy;

namespace gtdpad
{
    public static class Extensions
    {
        public static T SetDefaults<T>(this IModel model)
        {
            if(model.ID == Guid.Empty)
                model.ID = Guid.NewGuid();

            return (T)model;
        }

        public static GTDPadIdentity GetUser(this NancyModule module)
        {
            return (GTDPadIdentity)module.Context?.CurrentUser?.Identity;
        }
    }
}
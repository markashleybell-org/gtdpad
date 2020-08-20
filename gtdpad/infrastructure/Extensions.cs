using System;
using HtmlAgilityPack;
using Nancy;

namespace gtdpad
{
    public static class Extensions
    {
        public static T SetDefaults<T>(this IModel model)
        {
            if (model.ID == Guid.Empty)
            {
                model.ID = Guid.NewGuid();
            }

            return (T)model;
        }

        public static GTDPadIdentity GetUser(this NancyModule module) =>
            (GTDPadIdentity)module.Context?.CurrentUser?.Identity;

        public static string GetText(this HtmlDocument html, string xpath) =>
            html.DocumentNode.SelectSingleNode(xpath)?.Attributes["content"]?.Value;
    }
}

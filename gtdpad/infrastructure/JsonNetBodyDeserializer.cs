using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using gtdpad.infrastructure;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;

namespace gtdpad
{
    public class JsonNetBodyDeserializer : IBodyDeserializer
    {
        private readonly JsonSerializer serializer;

        public JsonNetBodyDeserializer() =>
            serializer = JsonSerializer.CreateDefault();

        public JsonNetBodyDeserializer(JsonSerializer serializer) =>
            this.serializer = serializer;

        public bool CanDeserialize(MediaRange mediaRange, BindingContext context) =>
            Helpers.IsJsonType(mediaRange);

        public object Deserialize(MediaRange mediaRange, Stream bodyStream, BindingContext context)
        {
            if (bodyStream.CanSeek)
            {
                bodyStream.Position = 0;
            }

            var deserializedObject =
                serializer.Deserialize(new StreamReader(bodyStream), context.DestinationType);

            var properties =
                context.DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new BindingMemberInfo(p));

            var fields =
                context.DestinationType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Select(f => new BindingMemberInfo(f));

            if (properties.Concat(fields).Except(context.ValidModelBindingMembers).Any())
            {
                return CreateObjectWithBlacklistExcluded(context, deserializedObject);
            }

            return deserializedObject;
        }

        private static object ConvertCollection(object items, Type destinationType, BindingContext _)
        {
            var returnCollection = Activator.CreateInstance(destinationType);

            var collectionAddMethod =
                destinationType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

            foreach (var item in (IEnumerable)items)
            {
                collectionAddMethod.Invoke(returnCollection, new[] { item });
            }

            return returnCollection;
        }

        private static object CreateObjectWithBlacklistExcluded(BindingContext context, object deserializedObject)
        {
            var returnObject = Activator.CreateInstance(context.DestinationType, true);

            if (context.DestinationType.IsCollection())
            {
                return ConvertCollection(deserializedObject, context.DestinationType, context);
            }

            foreach (var property in context.ValidModelBindingMembers)
            {
                CopyPropertyValue(property, deserializedObject, returnObject);
            }

            return returnObject;
        }

        private static void CopyPropertyValue(BindingMemberInfo property, object sourceObject, object destinationObject) =>
            property.SetValue(destinationObject, property.GetValue(sourceObject));
    }
}

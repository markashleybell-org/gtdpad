using System.Collections.Generic;
using System.IO;
using gtdpad.infrastructure;
using Nancy;
using Nancy.IO;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace gtdpad
{
    public class JsonNetSerializer : ISerializer
    {
        private readonly JsonSerializer serializer;

        public JsonNetSerializer()
        {
            serializer = JsonSerializer.CreateDefault();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializer.Formatting = Formatting.Indented;
        }

        public JsonNetSerializer(JsonSerializer serializer)
        {
            this.serializer = serializer;
            this.serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            this.serializer.Formatting = Formatting.Indented;
        }

        public IEnumerable<string> Extensions
        {
            get { yield return "json"; }
        }

        public bool CanSerialize(MediaRange mediaRange) =>
            Helpers.IsJsonType(mediaRange);

        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            using var writer = new JsonTextWriter(new StreamWriter(new UnclosableStreamWrapper(outputStream)));
            serializer.Serialize(writer, model);
        }
    }
}

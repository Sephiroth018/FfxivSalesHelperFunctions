using System;
using Newtonsoft.Json;

namespace Ffxiv.Common
{
    public class StringJsonConverter : JsonConverter<long>
    {
        public override void WriteJson(JsonWriter writer, long value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override long ReadJson(JsonReader reader, Type objectType, long existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return long.Parse(reader.Value.ToString());
        }
    }
}
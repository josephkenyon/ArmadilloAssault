using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Converters
{
    public class FloatListConverter : JsonConverter<List<float>>
    {
        public override void WriteJson(JsonWriter writer, List<float> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (float floatValue in value)
            {
                float roundedValue = (float)Math.Round(floatValue, 2);
                writer.WriteValue(roundedValue);
            }
            writer.WriteEndArray();
        }

        public override List<float> ReadJson(JsonReader reader, Type objectType, List<float> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("CustomFloatListConverter is only for serialization, not deserialization.");
        }

        public override bool CanRead
        {
            get { return false; }
        }
    }
}

using Newtonsoft.Json;
using System;

namespace ArmadilloAssault.Web.Converters
{
    public class FloatConverter : JsonConverter<float>
    {
        public override void WriteJson(JsonWriter writer, float value, JsonSerializer serializer)
        {
            writer.WriteValue((float)Math.Round(value, 2));
        }

        public override float ReadJson(JsonReader reader, Type objectType, float existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("CustomFloatListConverter is only for serialization, not deserialization.");
        }

        public override bool CanRead
        {
            get { return false; }
        }
    }
}

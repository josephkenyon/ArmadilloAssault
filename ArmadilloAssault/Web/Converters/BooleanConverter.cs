using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Converters
{
    public class BooleanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(List<bool>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return objectType == typeof(List<bool>) ? new List<bool>() : false;

            if (objectType == typeof(List<bool>))
            {
                var list = new List<bool>();

                foreach (var item in (List<int>)serializer.Deserialize(reader, typeof(List<int>)))
                {
                    list.Add(item == 1);
                }

                return list;
            }
            else
            {
                return Convert.ToInt32(reader.Value) == 1;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is List<bool>)
            {
                var list = (List<bool>)value;
                writer.WriteStartArray();
                foreach (var item in list)
                {
                    writer.WriteValue(item ? 1 : 0);
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteValue((bool)value ? 1 : 0);
            }
        }
    }
}

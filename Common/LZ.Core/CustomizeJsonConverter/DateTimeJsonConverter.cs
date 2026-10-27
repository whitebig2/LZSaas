using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace LZ.Core.CustomizeJsonConverter
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) { return DateTime.Parse(reader.GetString()); }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) { writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss")); }
    }

}

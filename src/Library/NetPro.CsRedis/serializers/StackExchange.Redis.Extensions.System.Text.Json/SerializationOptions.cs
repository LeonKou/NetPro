using System.Text.Encodings.Web;
using System.Text.Json;

namespace NetPro.CsRedis
{
    internal static class SerializationOptions
    {
        static SerializationOptions()
        {
            Flexible = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
#if NET6_0_OR_GREATER
                 DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
#else
                IgnoreNullValues = true,
#endif
                WriteIndented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            Flexible.Converters.Add(new StringToIntCustomConverter());
            Flexible.Converters.Add(new CultureCustomConverter());
            Flexible.Converters.Add(new TimezoneCustomConverter());
            Flexible.Converters.Add(new TimeSpanConverter());
        }

        public static JsonSerializerOptions Flexible { get; private set; }
    }
}

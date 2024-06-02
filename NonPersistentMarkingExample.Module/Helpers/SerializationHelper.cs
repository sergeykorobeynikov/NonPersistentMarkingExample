using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace NonPersistentMarkingExample.Module.Helpers
{
    internal static class SerializationHelper
    {
        static readonly JsonSerializerOptions jso;

        static SerializationHelper()
        {
            jso = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }

        public static string Serialize<T>(T obj)
            where T : class, new()
        {
            return JsonSerializer.Serialize<T>(obj, jso);
        }

        public static T Deserialize<T>(string json)
            where T : class, new()
        {
            return JsonSerializer.Deserialize<T>(json, jso);
        }

        public static bool TryDeserialize<T>(string json, out T obj)
            where T : class, new()  
        {
            obj = null;

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    obj = JsonSerializer.Deserialize<T>(json, jso);
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}

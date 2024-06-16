using Newtonsoft.Json;

namespace Unity2Debug.Common.Utility
{
    internal class Json
    {
        public static string ToJSON<T>(T input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public static T? FromJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}

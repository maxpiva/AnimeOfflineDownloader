using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ADBaseLibrary
{
    public class SessionDictionarySerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<string, ISession> input = value as Dictionary<string, ISession>;
            Dictionary<string, Dictionary<string, string>> serializedsessions = new Dictionary<string, Dictionary<string, string>>();
            if (input != null)
            {
                foreach (string k in input.Keys)
                    serializedsessions.Add(k, input[k].Serialize());
            }
            serializer.Serialize(writer,serializedsessions);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<string, ISession> sessions = new Dictionary<string, ISession>();
            Dictionary<string, Dictionary<string, string>> data = serializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(reader);
            foreach (string k in data.Keys)
            {
                if (DownloadPluginHandler.Instance.Plugins.ContainsKey(k))
                    sessions.Add(k, DownloadPluginHandler.Instance.Plugins[k].Deserialize(data[k]));
            }
            return sessions;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Dictionary<string, ISession>).IsAssignableFrom(objectType);

        }
    }
}

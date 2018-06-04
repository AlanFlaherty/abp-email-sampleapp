using Newtonsoft.Json;
using System.Collections.Generic;
using FileContextCore.Serializer;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore.FileContext
{
    public class AbpDataEntityJSONSerializer : ISerializer
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ContractResolver = new AbpDataEntityResolver()
        };

        public AbpDataEntityJSONSerializer(Formatting formatting = Formatting.Indented)
        {
            settings.Formatting = formatting;
        }

        public string FileType { get { return "json"; } }

        public List<T> DeserializeList<T>(string list)
        {
            return JsonConvert.DeserializeObject<List<T>>(list, settings);
        }

        public string SerializeList<T>(List<T> list)
        {
            return JsonConvert.SerializeObject(list, settings);
        }

        public T Deserialize<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj, settings);
        }

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}

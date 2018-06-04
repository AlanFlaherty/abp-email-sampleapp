using System;
using FileContextCore.Extensions;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

using System.Collections.Generic;
using FileContextCore.Serializer;

using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public static class AbpProjectNameDbContextConfigurer
    {
        private static bool IsTesting()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return environment.ToUpper() == "TESTING";
        }

        public static void Configure(DbContextOptionsBuilder<AbpProjectNameDbContext> builder, string connectionString)
        {
            if (IsTesting())
            {
                builder.UseFileContext(new MyJSONSerializer());
            }
            else
            {
                builder.UseSqlServer(connectionString,
                    options =>
                    {
                        options.UseRowNumberForPaging();
                        options.MigrationsHistoryTable(String.Format("__{0}MigrationsHistory", AbpProjectNameConsts.TablePrefix), AbpProjectNameConsts.Schema);
                    }
                );
            }
        }
    }

    public class MyJSONSerializer : ISerializer
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ContractResolver = new PreserveVirtualResolver()
        };

        public MyJSONSerializer(Formatting formatting = Formatting.Indented)
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

    class PreserveVirtualResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);
            PropertyInfo propInfo = (PropertyInfo)member;

            prop.Ignored = false;

            if (propInfo != null)
            {
                if (!propInfo.ShouldSerialize())
                {
                    prop.ShouldSerialize = obj => false;
                }
            }

            return prop;
        }
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization);
        }
    }

    static class PropertyHelper
    {
        public static PropertyInfo[] GetPropertiesForSerialize(this Type t)
        {
            return t.GetRuntimeProperties().Where(x =>
                (!x.GetMethod.IsVirtual || x.GetMethod.IsFinal) &&
                !x.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute)))
                .ToArray();
        }

        public static PropertyInfo[] GetPropertiesNotForSerialize(this Type t)
        {
            return t.GetRuntimeProperties().Where(x =>
                (x.GetMethod.IsVirtual && !x.GetMethod.IsFinal) ||
                x.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute)))
                .ToArray();
        }

        public static bool ShouldSerialize(this PropertyInfo prop)
        {
            return prop.GetMethod.IsVirtual && !prop.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute));
            // and not a navigation property


            // return (!prop.GetMethod.IsVirtual || prop.GetMethod.IsFinal) &&
            //    !prop.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute));
        }
    }
}
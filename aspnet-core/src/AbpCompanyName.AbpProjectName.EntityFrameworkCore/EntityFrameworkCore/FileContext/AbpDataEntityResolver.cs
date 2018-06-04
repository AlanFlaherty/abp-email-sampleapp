using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore.FileContext
{
    class AbpDataEntityResolver : DefaultContractResolver
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
}

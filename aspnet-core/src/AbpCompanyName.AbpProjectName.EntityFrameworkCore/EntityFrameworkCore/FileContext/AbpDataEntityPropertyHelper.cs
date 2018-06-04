using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore.FileContext
{
    static class AbpDataEntityPropertyHelper
    {
        public static bool ShouldSerialize(this PropertyInfo prop)
        {
            return prop.GetMethod.IsVirtual && !prop.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute));
        }
    }
}

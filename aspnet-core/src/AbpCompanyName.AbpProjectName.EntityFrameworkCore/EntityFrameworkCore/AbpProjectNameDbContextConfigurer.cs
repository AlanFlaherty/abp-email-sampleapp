using System;
using System.Data.Common;

using FileContextCore.Extensions;
using Microsoft.EntityFrameworkCore;

using AbpCompanyName.AbpProjectName.EntityFrameworkCore.FileContext;

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
                builder.UseFileContext(new AbpDataEntityJSONSerializer());
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

        public static void Configure(DbContextOptionsBuilder<AbpProjectNameDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}

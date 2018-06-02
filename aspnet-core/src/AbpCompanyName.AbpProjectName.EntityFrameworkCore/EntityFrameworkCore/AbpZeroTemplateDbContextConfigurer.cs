using System;

using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public static class AbpProjectNameDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AbpProjectNameDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString, 
                options=>
                {
                    options.UseRowNumberForPaging();
                    options.MigrationsHistoryTable(String.Format("__{0}MigrationsHistory", AbpProjectNameConsts.TablePrefix), AbpProjectNameConsts.Schema);
                }
            );
        }
    }
}
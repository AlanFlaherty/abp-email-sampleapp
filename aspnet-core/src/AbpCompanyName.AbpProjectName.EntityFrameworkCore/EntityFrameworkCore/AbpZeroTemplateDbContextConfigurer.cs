using AbpCompanyName.AbpProjectName.Configuration;
using AbpCompanyName.AbpProjectName.Web;
using Microsoft.EntityFrameworkCore;
using System;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public static class AbpProjectNameDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AbpProjectNameDbContext> builder, string connectionString)
        {
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());
            var prefix = configuration["Database:TablePrefix"];
            var schema = configuration["Database:Schema"];
            
            builder.UseSqlServer(connectionString, 
                options=>
                {
                    options.UseRowNumberForPaging();
                    options.MigrationsHistoryTable(String.Format("__{0}MigrationsHistory", prefix), schema);
                }
            );
        }
    }
}
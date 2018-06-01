using Abp.Authorization.Users;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AbpCompanyName.AbpProjectName.MultiTenancy;

using AbpCompanyName.AbpProjectName.Configuration;
//using AbpCompanyName.AbpProjectName.Email;
using AbpCompanyName.AbpProjectName.Web;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public class AbpProjectNameDbContext : AbpZeroDbContext<Tenant, Role, User, AbpProjectNameDbContext>
    {
        /* Define an IDbSet for each entity of the application */
//        public DbSet<EmailSettings> EmailSettings { get; set;}
        
        public AbpProjectNameDbContext(DbContextOptions<AbpProjectNameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());
            var prefix = configuration["Database:TablePrefix"];
            var schema = configuration["Database:Schema"];
            
            // Application tables
            //modelBuilder.Entity<EmailSettings>().ToTable(prefix + "EmailSettings", schema);

            // Abp tables
            modelBuilder.Entity<UserToken>().ToTable(prefix + "UserToken", schema);
            modelBuilder.ChangeAbpTablePrefix<Tenant, Role, User>(prefix, schema);
        }
    }
}

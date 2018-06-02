using Abp.Authorization.Users;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AbpCompanyName.AbpProjectName.MultiTenancy;

//using AbpCompanyName.AbpProjectName.Email;

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
            
            // Application tables
            //modelBuilder.Entity<EmailSettings>().ToTable(AbpProjectNameConsts.TablePrefix + "EmailSettings", AbpProjectNameConsts.Schema);

            // Abp tables
            modelBuilder.Entity<UserToken>().ToTable(AbpProjectNameConsts.TablePrefix + "UserToken", AbpProjectNameConsts.Schema);
            modelBuilder.ChangeAbpTablePrefix<Tenant, Role, User>(AbpProjectNameConsts.TablePrefix, AbpProjectNameConsts.Schema);
        }
    }
}

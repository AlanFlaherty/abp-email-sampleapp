namespace AbpCompanyName.AbpProjectName
{
    public class AbpProjectNameConsts
    {
        public const string LocalizationSourceName = "AbpProjectName";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;

        // Change 'Schema' to match he branch to allow more than one appication
        // to share the same Database. Possibly have to generate the schema
        // and apply manually to the sql server after initial deployment:
        //  dotnet ef migrate script -o migration.sql
        public const string Schema = "master";
        
        public const string TablePrefix = "";
    }
}
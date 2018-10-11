
namespace AbpCompanyName.AbpProjectName.Emails
{
    public class ValidateAccountOutput
    {
        public string Status {get; private set;} 

        public ValidateAccountOutput(string status)
        {
            this.Status = status;
        }
    }
}
using System;
using System.Threading.Tasks;

using MailKit.Security;

namespace AbpCompanyName.AbpProjectName.Core.Emails
{
    
    //TODO: typescript dtos...
    public class AccountDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string IMapServer { get; set; }
        public int IMapPort { get; set; }
        public SecureSocketOptions IMapSocketSecurity { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public SecureSocketOptions SmtpSocketSecurity { get; set; }
    }
}
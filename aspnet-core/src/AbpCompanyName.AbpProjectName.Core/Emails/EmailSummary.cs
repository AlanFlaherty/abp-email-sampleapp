using System;
using System.Threading.Tasks;

using MailKit;

namespace AbpCompanyName.AbpProjectName.Core.Emails
{
    //TODO: typescript dtos...
    public class EmailSummary
    {
        public UniqueId UID { get; set; }
        public String From  { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
    }
}
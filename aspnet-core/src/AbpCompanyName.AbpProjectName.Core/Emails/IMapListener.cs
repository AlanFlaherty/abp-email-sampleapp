using System;
using System.Threading.Tasks;

using MailKit.Security;

namespace AbpCompanyName.AbpProjectName.Core.Emails
{
    //TODO: refactor this class this as soon as possible

    public class IMapListener 
    {
        // SignalR connection id
        public String connectionID { get; set; }

        public String folder { get; set; }
        
        public String username { get; set; }
        
        public String password { get; set; }
        
        public String address { get; set; }
        
        public int port { get; set; }

        // TODO: remove MailKit from this leve as soon as possible
        public SecureSocketOptions secureSocketOptions { get; set; }
    }
}
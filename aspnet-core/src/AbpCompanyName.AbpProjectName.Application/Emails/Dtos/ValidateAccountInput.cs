
namespace AbpCompanyName.AbpProjectName.Emails
{
    public class ValidateAccountInput
    {
        // TODO: get rid of these if not required
        public string SourceComponentGuid{get; set;}
        
        public string SignalRConnectionID{get; set;}
        
        public string Username {get; set;}
        public string Password {get; set;}

        public string IMapServerName {get; set;}  
        public int IMapServerPort {get; set;}
        public string IMapSecurityType {get; set;}

        public string SmtpServerName {get; set;}  
        public int SmtpServerPort {get; set;}
        public string SmtpSecurityType {get; set;}

        // authenticate smtp..... or just assume authenticate... 
        // 
    }
}
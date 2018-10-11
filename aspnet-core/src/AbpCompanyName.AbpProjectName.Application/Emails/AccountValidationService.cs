using System.Threading.Tasks;
using Abp.Application.Services;

using MailKit;

using AbpCompanyName.AbpProjectName;
using AbpCompanyName.AbpProjectName.Core.Emails.Hubs;
using MailKit.Net.Imap;
using MailKit.Security;
using System;
using MailKit.Net.Smtp;

namespace AbpCompanyName.AbpProjectName.Emails
{
    public enum ValidationMessageStatus
    {
        Failure,        // Contents indicate a validation failure.
        Success,        // Contents indicate a validation success.
        StatusUpdate    // Contents just indicate an update.
    }

    public class AccountValidationService : AbpProjectNameAppServiceBase, IAccountValidationService
    {
        private IMailHub _mailHub;

        public AccountValidationService(IMailHub mailHub){
            _mailHub = mailHub;
        }

        public async Task<ValidateAccountOutput> ValidateAccount(ValidateAccountInput input)
        {
            await sendUpdateMessage("Authenticating Acount Details", 0, ValidationMessageStatus.StatusUpdate, input);
            await sendUpdateMessage("* Connecting to server", 10, ValidationMessageStatus.StatusUpdate, input);

            ImapClient client = new ImapClient();            
            try
            {
                await client.ConnectAsync(input.IMapServerName, input.IMapServerPort, Enum.Parse<SecureSocketOptions>(input.IMapSecurityType));
            }
            catch(Exception ex)
            {
                await sendUpdateMessage("* Exception thrown in connect" , 10, ValidationMessageStatus.StatusUpdate, input);
                await sendUpdateMessage("* Message: " + ex.Message,  100, ValidationMessageStatus.Failure, input);

                return new ValidateAccountOutput("Failure");
            }

            await sendUpdateMessage("* Attempting Imap Authentication", 30, ValidationMessageStatus.StatusUpdate, input);
            try
            {
                await client.AuthenticateAsync(input.Username, input.Password);
            }
            catch(Exception ex)
            {
                await sendUpdateMessage("* Exception thrown in authenticate" , 10, ValidationMessageStatus.StatusUpdate, input);
                await sendUpdateMessage("* Message: " + ex.Message,  100, ValidationMessageStatus.Failure, input);

                return new ValidateAccountOutput("Failure");
            }

            await sendUpdateMessage("* Opening Inbox", 40, ValidationMessageStatus.StatusUpdate, input);
            try
            {
                await client.Inbox.OpenAsync(FolderAccess.ReadOnly);
            }
            catch(Exception ex)
            {
                await sendUpdateMessage("* Exception thrown in authenticate" , 10, ValidationMessageStatus.StatusUpdate, input);
                await sendUpdateMessage("* Message: " + ex.Message,  100, ValidationMessageStatus.Failure, input);

                return new ValidateAccountOutput("Failure");
            }
            await sendUpdateMessage("* Success" , 50, ValidationMessageStatus.StatusUpdate, input);
            await sendUpdateMessage("* " + client.Inbox.Unread + " unread messages in inbox", 25, ValidationMessageStatus.StatusUpdate, input);

            SmtpClient smtpClient = new SmtpClient();
            await sendUpdateMessage("* Connecting to SMTP Server" , 60, ValidationMessageStatus.StatusUpdate, input);
            try
            {
                await smtpClient.ConnectAsync(input.SmtpServerName);
            }
            catch(Exception ex)
            {
                await sendUpdateMessage("* Exception thrown in connect" , 60, ValidationMessageStatus.StatusUpdate, input);
                await sendUpdateMessage("* Message: " + ex.Message,  100, ValidationMessageStatus.Failure, input);

                return new ValidateAccountOutput("Failure");
            }

            await sendUpdateMessage("* Attempting Smtp Authentication" , 80, ValidationMessageStatus.StatusUpdate, input);
            try
            {
                await smtpClient.AuthenticateAsync(input.Username, input.Password);
            }
            catch(Exception ex)
            {
                await sendUpdateMessage("* Exception thrown in smtp autheticate" , 10, ValidationMessageStatus.StatusUpdate, input);
                await sendUpdateMessage("* Message: " + ex.Message,  100, ValidationMessageStatus.Failure, input);

                return new ValidateAccountOutput("Failure");
            }
            await sendUpdateMessage("* Success" , 90, ValidationMessageStatus.StatusUpdate, input);
            await sendUpdateMessage("* Account Details Validated", 100, ValidationMessageStatus.Success, input);
        
            return new ValidateAccountOutput("Success");
        }

        private async Task sendUpdateMessage(string message, int percentageComplete, ValidationMessageStatus status, ValidateAccountInput input)
        {
            await _mailHub.SendServiceProgressMessage(input.SourceComponentGuid, message, percentageComplete, status.ToString());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbpCompanyName.AbpProjectName.Web.Host.BackgroundTasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using AbpCompanyName.AbpProjectName.Core.Emails.Hubs;
using AbpCompanyName.AbpProjectName.Core.Emails;

namespace AbpCompanyName.AbpProjectName.Web.Host.Hubs
{
    public class MailHub : Hub<IMailClientHub>, IMailHub
    {
         readonly ILogger<MailHub> log;

        private IMAPHostedService imapService;

        public MailHub(IMAPHostedService imapService, ILogger<MailHub> log)
        {
            this.imapService = imapService;
            this.log = log;
        }

        public Task SendMessage(string message)
        {
            checkContext("SendMessage");
            // Clients.
            return Clients.All.ReceiveMessage(message);
        }

        public async Task SendServiceProgressMessage(string sourceComponentGuid,  string message, int percentageComplete, string status)
        {
            log.LogInformation("MailHub:: SendServiceProgressMessage {0}% ({1})", percentageComplete, status);

            if(Context == null)
            {
                log.LogInformation("Context is null");
                return;
            }

            IMailClientHub client = Clients.Client(Context.ConnectionId);

            if(client == null)
            {
                log.LogInformation("No Client Connected... {0}", Context.ConnectionId);
            }
            else
            {
                await client.ReceiveServiceProgressMessage(sourceComponentGuid, message, percentageComplete, status);
            }
        }

        public async Task ChangeFolder(string folderName)
        {
            checkContext("ChangeFolder");

            if(Context == null)
            {
                log.LogInformation("Context is null");
                return;
            }

            await imapService.ChangeFolder(Context.ConnectionId, folderName);
        }
        
        public async Task ChangePage(int pageNumber)
        {
            checkContext("ChangePage");

            await imapService.ChangePage(Context.ConnectionId, pageNumber);
        }
        
        public async Task RegisterIMapListener(IMapListener listener)
        {
            checkContext("RegisterImapListener");

            listener.connectionID = Context.ConnectionId;
            await imapService.Register(listener);
        }

        public override async Task OnConnectedAsync()
        {
            log.LogInformation("Client '{0}' Connected ", Context.ConnectionId);
            
            // TODO: Make better use of this group in the code... 
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            log.LogInformation("Client '{0}' Disconnected ", Context.ConnectionId);
            
            Console.WriteLine("!!##!! Connection {0} disconnected", Context.ConnectionId);
            Console.WriteLine("!!##!! Calling UnRegister on the imapService");

            await imapService.UnRegister(Context.ConnectionId); 

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }

        private void checkContext(string source)
        {
            if(Context == null)
            {
                log.LogError("{0} Context is null", source);
                return;
            }
        }
    }
}
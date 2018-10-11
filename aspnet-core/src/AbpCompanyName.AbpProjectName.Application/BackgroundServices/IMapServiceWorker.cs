using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MailKit;

using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;

using Microsoft.Extensions.Logging;

using AbpCompanyName.AbpProjectName.Core.Emails;

namespace AbpCompanyName.AbpProjectName.Application.BackgroundServices
{

    public class ImapServiceWorker
    {
        // Leak?? SignalR ConnectionID
        public String ConnectionID{get; private set;}
        public AccountDetails Account{get; set;}
        public event EventHandler<ImapUpdateEventArgs> ImapUpdate;   
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public int PageNumber { get; set; }

        private readonly ILogger<ImapServiceWorker> _log;

        private ImapClient client;

        public string FolderName { get; set; }

        private IMailFolder activeFolder;

        private int NextMessageIndex;
        
        private int pageSize = 20;

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);

        protected virtual void OnImapUpdate(ImapUpdateEventArgs e)
        {
            if (ImapUpdate != null)
            {
                ImapUpdate(this, e);
            }
        }

        protected void SendUpdate(UpdateDetails details)
        {
            OnImapUpdate(new ImapUpdateEventArgs(ConnectionID, details));
        }

        public ImapServiceWorker(AccountDetails account, ILogger<ImapServiceWorker> log) : this(account, Guid.NewGuid().ToString(), log)
        {
        }

        public ImapServiceWorker(AccountDetails account, String connectionID, ILogger<ImapServiceWorker> log)
        {
            this.Account = account;
            this.ConnectionID = connectionID;
            this.PageNumber = 1;

            // this.client = new ImapClient(new ProtocolLogger("YandexIdle.txt"));
            this.client = new ImapClient();

            // TODO: Avoid this if possible or let the end user know and have to approve it.
/*
            this.client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                // TODO: Check the validation results only errors for private certs
                Console.WriteLine("Overriding ssl cert validation, accept all certificates. Mail servers with private certs..");
                Console.WriteLine("Issuer: {0}", certificate.Issuer);
                Console.WriteLine("Subject: {0}", certificate.Subject);

                Console.WriteLine("Policy Errors: {0}", sslPolicyErrors.ToString());

                return true;
            };
 */
 
            this._log = log;
        }

        public async Task Reset(CancellationTokenSource newTokenSource){
            CancellationTokenSource.Cancel();
            _log.LogInformation("Reset semaphoreSlim.WaitAsync");
            await semaphoreSlim.WaitAsync();
            _log.LogInformation("Reset semaphoreSlim.WaitAsync Completed");
            CancellationTokenSource = newTokenSource;

            semaphoreSlim.Release();
        }

        public async Task Cancel(){
            CancellationTokenSource.Cancel();

            _log.LogInformation("Cancel semaphoreSlim.WaitAsync");
            await semaphoreSlim.WaitAsync();
            _log.LogInformation("Cancel semaphoreSlim.WaitAsync Completed");

            await client.DisconnectAsync(true);

            // TODO: check if dispose pattern is required in ImapServiceWorker
            client.Dispose();

            // Should not need to release semaphore as object should be disposed after this call.. 
            // TODO: Double check this doesn't cause issues in the framework waiting for disposes etc..
            // semaphoreSlim.Release();
        }

        public async Task ChangePage(int pageNumber, CancellationTokenSource tokenSource)
        {
            _log.LogInformation("Watch semaphoreSlim.WaitAsync");
            await semaphoreSlim.WaitAsync();
            _log.LogInformation("Watch semaphoreSlim.WaitAsync Completed");

            // 
        }

        public async Task Watch(string folderName, CancellationTokenSource tokenSource)
        {
            _log.LogInformation("Watch semaphoreSlim.WaitAsync");
            await semaphoreSlim.WaitAsync();
            _log.LogInformation("Watch semaphoreSlim.WaitAsync Completed");

            try
            {            
                NextMessageIndex = 0;
                FolderName = folderName;

                if(!client.IsAuthenticated)
                {
                    await ConnectAndAuthenticate(Account, tokenSource.Token);
                }

                if(activeFolder != null && activeFolder.IsOpen)
                {
                    try
					{
                        await activeFolder.CloseAsync();
                    }
                    catch(Exception ex)
					{
                        Console.WriteLine("Watch activeFolder.CloseAsync threw exception: {0}", ex.Message);
                    }
                }
                activeFolder = await client.GetFolderAsync(FolderName, tokenSource.Token);

                await activeFolder.OpenAsync(FolderAccess.ReadOnly);
                Console.WriteLine("Watch !ActiveFolder [{0}] IsOpen: {1}", activeFolder.FullName, activeFolder.IsOpen);

                await AsyncLoop(tokenSource.Token);

                Console.WriteLine("Watch Exit Async Loop, token cancelled: {0}", tokenSource.Token.IsCancellationRequested);
            }
            finally
			{
                _log.LogInformation("Watch semaphoreSlim.Release()");
                semaphoreSlim.Release();
                _log.LogInformation("Watch semaphoreSlim.Release() Completed");
            }
        }

        private async Task AsyncLoop(CancellationToken token)
        {
            try
            {
                await CreateUpdate(token);

                while (!token.IsCancellationRequested)
                {
                    await Idle(token);
                    await CreateUpdate(token);
                }

                Console.WriteLine("AsyncLoop Exiting , token.IsCancellationRequested : {0}", token.IsCancellationRequested);
            }
            catch(Exception ex)
            {
                Console.WriteLine("AsyncLoop Exiting threw {0}: {1} \r\n{2}",ex.GetType().Name,ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        private async Task Idle(CancellationToken token)
        {
            if(token.IsCancellationRequested)
            {
                Console.WriteLine("Idle Called with cancelled token");
                return;
            }

            Console.WriteLine("Idle ****** Idling *********");
            CancellationTokenSource idleTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            EventHandler<EventArgs> onCountChanged = (source, eventargs) =>
            {
                Console.Write("Idle onCountChanged Folder '{0}' count changed..", FolderName);
                // need to cancel out of this an re run both previous tasks.
                // Update both the folder details and update the paged listings
                // Just send new messages to client or refresh the data set that they have
                idleTokenSource.Cancel();
                Console.WriteLine("# Cancelling Idle..");
            };

            // lock here...
            activeFolder.CountChanged += onCountChanged;

            bool reconnectException = true;
            while(!idleTokenSource.Token.IsCancellationRequested && reconnectException)
            {
                try
                {
                    // TODO: avoid Multiple subscription..
                    client.Disconnected += (sender, eventArgs)=>{
                        // Reconnect
                        Console.WriteLine("Idle Client disconnected");

                        // TODO: Implement the disconnect/reconnect case... very rarely happens in testing.. 
                        // but likely to happen after connections are active for a few idle loops. ..And seems to 
                        // reconnect as is... 
                    };
                    await client.IdleAsync(idleTokenSource.Token);
                }
                catch(Exception ex)
                {
                    reconnectException = ex.Message.StartsWith("Unable to read data from the transport connection");
                    Console.WriteLine("Idle {0}: {1} \r\n{2}", ex.GetType().Name, ex.Message, ex.StackTrace);
                }
            }

            Console.WriteLine("Idle #Token cancelled. ");

            // do we need to lock here...
            activeFolder.CountChanged -= onCountChanged;
            Console.WriteLine("Idle #Exit Idle. ");
        }

        private async Task CreateUpdate(CancellationToken token)
        {
            if(token.IsCancellationRequested)
            {
                Console.WriteLine("CreateUpdate Called with cancelled token");
                return;
            }

            // 
            UpdateDetails details = new UpdateDetails();
            details.FolderName = FolderName;

            // Update These on client
            details.MesssageCount = activeFolder.Count;
            details.RecentMessageCount = activeFolder.Recent;
            details.UnreadMessageCount = activeFolder.Unread;

            // TODO: Add PageSize
            details.PageNumber = this.PageNumber;

            IList<IMessageSummary> messages = null;
            int attempt = 1;
            while(attempt < 10) // TODO: CHECK this, though this was the suggested pattern by the library developers
            {
                try
                {
                    // get ids only then page the results and fetch summary items for the page
                    IList<UniqueId> uids = await activeFolder.SearchAsync(SearchQuery.All);

                    // take a page of these uids, reverse the paging so that newest are on page one.
                    IList<UniqueId> pageOfUIds = uids.Reverse().Skip((PageNumber - 1) * pageSize).Take(pageSize).ToList();

                    // write to debug
                    string uidlist = String.Join( " ", pageOfUIds.Select(e => e.Id.ToString()).ToArray() );
                    Console.WriteLine("Page {0}, Uids: {1}", PageNumber,  uidlist);
                    
                    // fetching seems to revert the order to inorder rather than newest first, should test a random ordering of uids, Update package... grumble..
                    messages = await activeFolder.FetchAsync(pageOfUIds, MessageSummaryItems.Full | MessageSummaryItems.UniqueId, token);

                    // newest first paging for mail...
                    messages = messages.Reverse().ToList();

                    // check order from server is not reverted to inbox order
                    string fetchOrderUidlist = String.Join(" ", messages.Select(e => e.UniqueId.Id.ToString()).ToArray() );
                    Console.WriteLine("FetchOrder Uids: {0}", fetchOrderUidlist);

                    // Note: Uses index on the older 1.22 library. 2.0+ uses the uid for the index....
                    // messages = await activeFolder.FetchAsync(NextMessageIndex, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId, token);

                    break;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("CreateUpdate Folder [isNull: {3}] not open: Attempt: {0}, EX: {1}\r\n{2}",  attempt, ex.Message, ex.StackTrace, activeFolder == null);

                    if(ex.Message == "The folder is not currently open.")
                    {
                        attempt++;
                        Console.WriteLine("CreateUpdate 'The folder is not currently open' error number {0}", attempt);
                        await Task.Delay(50);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            IList<EmailSummary> emails = new List<EmailSummary>();
            foreach(IMessageSummary message in messages)
            {
                emails.Add(new EmailSummary(){
                    UID=message.UniqueId,
                    From=message.Envelope.From.ToString(),
                    Subject=message.NormalizedSubject,
                    Date=message.Envelope.Date.Value.DateTime
                 });
            }
            details.Messages=emails;

            if(messages.Count > 0)
            {
                // TODO: Should this be reverse arrival to order by uid...
                NextMessageIndex = messages.Sort(new List<OrderBy>{OrderBy.ReverseDate})[0].Index + 1;
            }

            SendUpdate(details);
        }

        private async Task ConnectAndAuthenticate(AccountDetails account, CancellationToken token)
        {
            // TODO: All these errors need to be propigated back to the client in some way

            Console.WriteLine("ConnectAndAuthenticate **Connecting client");
            try
            {
                await client.ConnectAsync(account.IMapServer, account.IMapPort, account.IMapSocketSecurity, token);
            }
            catch(AuthenticationException ex)
            {
                Console.WriteLine("::ConnectAsync AuthenticationException: {0}", ex.Message);
                throw;
            }
            catch(SslHandshakeException ex)
            {
                Console.WriteLine("::ConnectAsync SslHandshakeException: {0}", ex.Message);
                throw;
            }
            catch(Exception ex)
            {
                Console.WriteLine("::ConnectAsync {0}: {1}", ex.GetType().Name, ex.Message);
                throw;
            }

            Console.WriteLine("ConnectAndAuthenticate **Authenticating client");
            try
            {
                await client.AuthenticateAsync(account.Username, account.Password, token);
            }
            catch(Exception ex)
            {
                Console.WriteLine("::AuthenticateAsync {0}: {1}", ex.GetType().Name, ex.Message);
                throw;                
            }

            Console.WriteLine("::ConnectAndAuthenticate **Client connected and authenticated");
        }
    }
}
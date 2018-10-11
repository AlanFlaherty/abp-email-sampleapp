using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using MailKit;
using MailKit.Search;
using MailKit.Security;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;

using AbpCompanyName.AbpProjectName.Web.Host.Hubs;
using Microsoft.Extensions.Hosting;

// using Spike;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using AbpCompanyName.AbpProjectName.Core.Emails;
using AbpCompanyName.AbpProjectName.Core.Emails.Hubs;

using AbpCompanyName.AbpProjectName.Application.BackgroundServices;

namespace AbpCompanyName.AbpProjectName.Web.Host.BackgroundTasks
{
    public interface IIMapHostedService: IHostedService
    {
        Task Register(IMapListener listener);

        Task UnRegister(string connectionID);

        Task ChangeFolder(string connectionID, String folderName);   
    }

    public class IMAPHostedService : HostedService, IIMapHostedService, IDisposable
    {
        private object lockObject;
        private readonly ILogger<IMAPHostedService> _log;
		private IHubContext<MailHub, IMailClientHub> _hub;
        private List<ImapServiceWorker> _serviceWorkers = new List<ImapServiceWorker>();

        // TODO: check this..
        private IServiceProvider _serviceProvider;

        public IMAPHostedService(IHubContext<MailHub, IMailClientHub> hub,  ILogger<IMAPHostedService> log, IServiceProvider serviceProvider) // this is context here rather than hub.. safe to inject hosted service into the hub???.. err
        {
            lockObject = new object();
            _hub = hub;
            _log = log;
            _serviceProvider = serviceProvider;
        }

        public async Task Register(IMapListener listener)
        {
            bool existing = _serviceWorkers.Exists(x=>x.ConnectionID == listener.connectionID) ;
            if(existing)
            {
                Console.WriteLine("Found existing connection on {0}, unregistering it.", listener.connectionID);
                await UnRegister(listener.connectionID);
            }

            Console.WriteLine("Register IMAPLIstener {0}", listener);

            AccountDetails details = new AccountDetails();
            details.IMapPort = listener.port;
            details.IMapServer = listener.address;
            details.IMapSocketSecurity = listener.secureSocketOptions;
            details.Password = listener.password;
            details.Username = listener.username;
            // TODO: add smtp details..

            ILoggerFactory loggerfactory = _serviceProvider.GetService<ILoggerFactory>();

            // Create an imap worker for the details and pass it a logger
            ImapServiceWorker serviceWorker = new ImapServiceWorker(details, listener.connectionID, loggerfactory.CreateLogger<ImapServiceWorker>());

            // Should this be on an intermediate object, container for serviceworker
            serviceWorker.CancellationTokenSource = new CancellationTokenSource();

            // send resopnses from the service worker to the signalr  hub
            serviceWorker.ImapUpdate += (sender, eventArgs)=>{
                Console.WriteLine("Register serviceWorker.ImapUpdate Message Count: {0}", eventArgs.Update.Messages.Count);

                _hub.Clients.Client(eventArgs.ConnectionID).RecieveIMapUpdates(eventArgs.Update);
                // _hub.Clients.All.RecieveIMapUpdates(eventArgs.Update);
            };

            _log.LogInformation("::Register Locking lockObj for connectionID '{0}'", listener.connectionID);
            lock(lockObject)
            {
                _serviceWorkers.Add(serviceWorker);
            }
            _log.LogInformation("::Register Releasing lockObj for connectionID '{0}'", listener.connectionID);

            Task.Run(
                ()=> serviceWorker.Watch("Inbox", serviceWorker.CancellationTokenSource)
            );

            showConnectionsOnConsole();
        }

        public async Task ChangeFolder(string connectionID, String folderName)
        {
            ImapServiceWorker worker = _serviceWorkers.Find(x=>x.ConnectionID == connectionID);
            await worker.Reset(new CancellationTokenSource());

            Task.Run(()=>worker.Watch(folderName, worker.CancellationTokenSource));
        }

        public async Task ChangePage(string connectionID, int pageNumber)
        {
            Console.WriteLine("IMAPHostedService::ChangePage {0}", pageNumber);

            ImapServiceWorker worker = _serviceWorkers.Find(x=>x.ConnectionID == connectionID);
            await worker.Reset(new CancellationTokenSource());

            worker.PageNumber = pageNumber;

            Task.Run(
                ()=> worker.Watch(worker.FolderName, worker.CancellationTokenSource)
            );
        }

        public async Task UnRegister(string connectionID)
        {
            ImapServiceWorker worker = _serviceWorkers.Find(x=>x.ConnectionID == connectionID);

            if(worker == null)
            {
                Console.WriteLine("!!##!! ERROR: No worker found for connectionID {0}", connectionID);
            }
            else
            {
                Console.WriteLine("!!##!!  Found worker on {0} for {1}", connectionID, worker.Account.Username);
            }

            // Note: if client login hangs then this will be null when the system shuts down
            // 
            await worker.Cancel(); // TODO: cancel any action really, check implemenation of cancelation token here

            _log.LogInformation("::UnRegister Locking lockObj for connection '{0}'", connectionID);
            lock(lockObject)
            {
                _serviceWorkers.Remove(worker);
            }
            _log.LogInformation("::UnRegister Releasing lockObj for connection '{0}'", connectionID);

            // TODO: Check if dispose patern is more correct and we should explicitly dispose the worker
            // here after removing for the service worker collection, rather than in the cancel
            // worker.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("IMAPHostedService::ExecuteAsync token.IsCancellationRequested:{0}", cancellationToken.IsCancellationRequested);

            while(!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("IMAPHostedService::ExecuteAsync Loop");
                showConnectionsOnConsole();

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        private void showConnectionsOnConsole()
        {
            String message = "";
            foreach(ImapServiceWorker worker in _serviceWorkers)
            {
                message += "\r\n *** " + worker.Account.Username + " - IDLING connection: " +  worker.ConnectionID; 
            }

            Console.WriteLine(message);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
//                    _connectionManager.Dispose();
//                    _outputStream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~IMAPHostedService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class IMapAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string IMapServer { get; set; }
        public int IMapPort { get; set; }
        public SecureSocketOptions SecureOptions { get; set; }
    }
}
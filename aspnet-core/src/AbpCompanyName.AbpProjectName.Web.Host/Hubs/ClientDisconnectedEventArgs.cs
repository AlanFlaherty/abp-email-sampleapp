using System;

namespace AbpCompanyName.AbpProjectName.Web.Host.Hubs
{
    public class ClientDisconnectedEventArgs : EventArgs 
    {
        private readonly string connectionID;

        public String ConnectionID 
        { 
            get {return connectionID;}
        }

        public ClientDisconnectedEventArgs(string connectionID)
        {
            this.connectionID = connectionID;
        }
    }
}
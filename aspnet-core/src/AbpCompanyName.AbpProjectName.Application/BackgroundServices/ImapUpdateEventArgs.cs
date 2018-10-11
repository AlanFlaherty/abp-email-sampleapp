using System;
using AbpCompanyName.AbpProjectName.Core.Emails;

namespace AbpCompanyName.AbpProjectName.Application.BackgroundServices
{
    public class ImapUpdateEventArgs: EventArgs
    {
        public String ConnectionID {get; private set; }
        public UpdateDetails Update { get; private set; }

        public ImapUpdateEventArgs(String connectionID, UpdateDetails update)
        {
            this.Update = update;
            this.ConnectionID = connectionID;
        }
    }
}
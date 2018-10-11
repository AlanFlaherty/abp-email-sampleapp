
using System;
using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Core.Emails.Hubs
{
    public interface IMailClientHub
    {
        Task ReceiveMessage(string message);

        Task RecieveIMapUpdates(UpdateDetails update);

        Task ReceiveServiceProgressMessage(string sourceComponentGuid,  string message, int percentageComplete, string status);
    }
}
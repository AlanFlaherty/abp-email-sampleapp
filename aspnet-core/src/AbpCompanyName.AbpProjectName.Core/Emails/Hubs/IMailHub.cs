
using System;
using System.Threading.Tasks;

namespace AbpCompanyName.AbpProjectName.Core.Emails.Hubs
{
    public interface IMailHub
    {
        Task SendServiceProgressMessage(string sourceComponentGuid,  string message, int percentageComplete, string status);

        Task RegisterIMapListener(IMapListener listener);

        Task ChangeFolder(string folderName);

        Task ChangePage(int pageNumber);
    }
}
using System.Threading.Tasks;

using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace AbpCompanyName.AbpProjectName.Emails
{
    public interface IAccountValidationService : IApplicationService
    {
        Task<ValidateAccountOutput> ValidateAccount(ValidateAccountInput input);
    }
}
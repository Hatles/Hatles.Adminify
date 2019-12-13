using System.Threading.Tasks;
using Abp.Application.Services;
using Hatles.Adminify.Authorization.Accounts.Dto;

namespace Hatles.Adminify.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}

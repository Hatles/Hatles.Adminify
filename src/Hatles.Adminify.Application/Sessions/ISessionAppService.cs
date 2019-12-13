using System.Threading.Tasks;
using Abp.Application.Services;
using Hatles.Adminify.Sessions.Dto;

namespace Hatles.Adminify.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}

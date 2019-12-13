using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Hatles.Adminify.Entities.Dtos;

namespace Hatles.Adminify.Tasks
{
    public interface ITaskAppService : IApplicationService
    {
        Task<ListResultDto<TaskDto>> GetAll();
    }
}

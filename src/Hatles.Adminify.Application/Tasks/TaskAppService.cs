using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Task = Hatles.Adminify.Entities.EntityTypes.Task;

namespace Hatles.Adminify.Tasks
{
    [AbpAllowAnonymous]
    public class TaskAppService : AdminifyAppServiceBase, ITaskAppService
    {
        private IRepository<Task> _repository;

        public TaskAppService(IRepository<Task> repository, AdminifyAsyncCrudAppService<Task, TaskDto> appService)
        {
            _repository = repository;
        }
        
        public async Task<ListResultDto<TaskDto>> GetAll()
        {
            var tasks = await _repository
                .GetAll()
                // .WhereIf(input.State.HasValue, t => t.State == input.State.Value)
                .OrderByDescending(t => t.CreationTime)
                .ToListAsync();

            return new ListResultDto<TaskDto>(
                ObjectMapper.Map<List<TaskDto>>(tasks)
            );
        }
    }
}


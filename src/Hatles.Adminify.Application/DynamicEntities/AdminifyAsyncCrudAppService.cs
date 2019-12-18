using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hatles.Adminify.DynamicEntities
{
    public class AdminifyAsyncCrudAppService<TEntity, TEntityDto>
        : AdminifyAsyncCrudAppService<TEntity, TEntityDto, int>, IDynamicEntityService<TEntity>
        where TEntity : class, IEntity<int>
        where TEntityDto : IEntityDto<int>
    {
        public AdminifyAsyncCrudAppService(IRepository<TEntity, int> repository)
            : base(repository)
        {

        }
        
        public async Task<ListResultDto<TEntityDto>> GetAllTest()
        {
            var entities = await Repository
                .GetAll()
                // .WhereIf(input.State.HasValue, t => t.State == input.State.Value)
                .ToListAsync();

            return new ListResultDto<TEntityDto>(
                ObjectMapper.Map<List<TEntityDto>>(entities)
            );
        }
    }

    public class AdminifyAsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey>
        : AsyncCrudAppService<TEntity, TEntityDto, TPrimaryKey>, IDynamicEntityService<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        public AdminifyAsyncCrudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {

        }
    }
}
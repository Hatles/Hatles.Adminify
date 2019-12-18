using Abp.Domain.Entities;

namespace Hatles.Adminify.DynamicEntities
{
    public interface IDynamicEntityService<TEntity> : IDynamicEntityService<TEntity, int> 
        where TEntity : IEntity<int>
    {
        
    }
    
    public interface IDynamicEntityService<TEntity, TPrimaryKey>
        where TEntity: IEntity<TPrimaryKey>
    {
        
    }
}
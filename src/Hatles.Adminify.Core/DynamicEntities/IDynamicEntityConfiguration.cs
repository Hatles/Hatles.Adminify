using Microsoft.EntityFrameworkCore;

namespace Hatles.Adminify.DynamicEntities
{
    public interface IDynamicEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        
    }
}
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.Entities.EntityTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hatles.Adminify.Entities.EntityConfigurations
{
    public class TaskEntityConfiguration : IDynamicEntityConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            var test = "test";
        }
    }
}
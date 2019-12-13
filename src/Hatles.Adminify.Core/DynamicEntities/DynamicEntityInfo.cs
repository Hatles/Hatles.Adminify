using System;

namespace Hatles.Adminify.DynamicEntities
{
    public class DynamicEntityInfo
    {
        /// <summary>
        /// Type of the entity.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// DbContext type that has DbSet property.
        /// </summary>
        public Type EntityConfigurationType { get; private set; }

        public DynamicEntityInfo(Type entityType, Type entityConfigurationType)
        {
            EntityType = entityType;
            EntityConfigurationType = entityConfigurationType;
        }
    }
}
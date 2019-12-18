using System;

namespace Hatles.Adminify.DynamicEntities
{
    public class DynamicEntityField
    {
        public string Name { get; }
        public Type Type { get; }

        public DynamicEntityField(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
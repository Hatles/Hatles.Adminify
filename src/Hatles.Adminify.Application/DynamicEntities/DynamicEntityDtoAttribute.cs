using System;
using AutoMapAttribute = Abp.AutoMapper.AutoMapAttribute;

namespace Hatles.Adminify.DynamicEntities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DynamicEntityDtoAttribute : AutoMapAttribute
    {
        public DynamicEntityDtoAttribute(params Type[] targetTypes) : base(targetTypes)
        {
        }
    }
}
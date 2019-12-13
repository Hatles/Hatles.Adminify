using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.Entities.EntityTypes;

namespace Hatles.Adminify.Entities.Dtos
{
    [DynamicEntityDto(typeof(Task))]
    public class TaskDto : EntityDto, IHasCreationTime
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public TaskState State { get; set; }
    }
}
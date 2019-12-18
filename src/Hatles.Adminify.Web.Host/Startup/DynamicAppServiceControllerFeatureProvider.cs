using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Reflection.Extensions;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Hatles.Adminify.Web.Host.Startup
{
    public class DynamicAppServiceControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly DynamicEntityManager _dynamicEntityManager;

        public DynamicAppServiceControllerFeatureProvider(DynamicEntityManager dynamicEntityManager)
        {
            _dynamicEntityManager = dynamicEntityManager;
        }

        protected bool IsController(TypeInfo typeInfo)
        {
            var type = typeInfo.AsType();

            if (!typeof(IApplicationService).IsAssignableFrom(type) ||
                !typeInfo.IsPublic || typeInfo.IsAbstract || typeInfo.IsGenericType)
            {
                return false;
            }

            var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(typeInfo);

            if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(type))
            {
                return false;
            }

            return true;
        }
        
        /// <inheritdoc />
        public void PopulateFeature(
            IEnumerable<ApplicationPart> parts,
            ControllerFeature feature)
        {
            var thisAssembly = typeof(AdminifyApplicationModule).GetAssembly();
            var entityDtos = thisAssembly.GetTypes()
                .Where(t => t.GetCustomAttributes(typeof(DynamicEntityDtoAttribute), true).Length == 1).Select(t => (dto: t, attribute: (DynamicEntityDtoAttribute) t.GetCustomAttributes(typeof(DynamicEntityDtoAttribute)).Single())).ToList();

            var entityInfos = _dynamicEntityManager.GetDynamicEntityInfos();
            
            foreach (var entityInfo in entityInfos)
            {
                try
                {
                    var entityDto = entityDtos.Single(e => e.attribute.TargetTypes.Any(t => t == entityInfo.EntityType));
                    RegisterEntityTypeAsController(feature, entityInfo, entityDto.dto, AppServiceTypes.Default);
                }
                catch (Exception e)
                {
                    RegisterEntityTypeAsController(feature, entityInfo, _dynamicEntityManager.DynamicDtoType, AppServiceTypes.Default);
                }
            }
        }
        
        public void RegisterEntityTypeAsController(
            ControllerFeature feature,
            DynamicEntityInfo entityInfo,
            Type entityDto,
            AutoAppServiceTypesAttribute autoAppServiceTypesAttribute)
        {

            RegisterEntityTypeAsController(
                feature,
                entityInfo,
                entityDto,
                autoAppServiceTypesAttribute.AppServiceTypesImplementation,
                autoAppServiceTypesAttribute.AppServiceTypesImplementationWithPrimaryKey
            );
        }

        public void RegisterEntityTypeAsController(ControllerFeature feature,
            DynamicEntityInfo entityInfo,
            Type entityDto,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityInfo.EntityType);
            if (primaryKeyType == typeof(int))
            {
                var implType =  repositoryImplementation.MakeGenericType(entityInfo.EntityType, entityDto);
                feature.Controllers.Add(implType.GetTypeInfo());
            }
            else
            {
                var genericImplType = repositoryImplementationWithPrimaryKey.MakeGenericType(entityInfo.EntityType, entityDto, primaryKeyType);
                feature.Controllers.Add(genericImplType.GetTypeInfo());
            }
        }
    }
}
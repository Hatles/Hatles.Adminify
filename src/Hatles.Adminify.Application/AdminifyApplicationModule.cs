using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Hatles.Adminify.Authorization;
using Hatles.Adminify.DynamicEntities;

namespace Hatles.Adminify
{
    [DependsOn(
        typeof(AdminifyCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AdminifyApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AdminifyAuthorizationProvider>();
            Configuration.Modules.AbpAutoMapper().Configurators.Add(config =>
            {
                using (IScopedIocResolver scope = IocManager.CreateScope())
                {
                    Logger.Debug("Registering dynamic app mappings");
                    
                    var entityManager = scope.Resolve<DynamicEntityManager>();
                    config.CreateMap(entityManager.DynamicType, entityManager.DynamicDtoType).ReverseMap();
                }
            });;
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AdminifyApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);
            
            using (IScopedIocResolver scope = IocManager.CreateScope())
            {
                Logger.Debug("Registering dynamic app services");

                var entityDtos = thisAssembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(DynamicEntityDtoAttribute), true).Length == 1).Select(t => (dto: t, attribute: (DynamicEntityDtoAttribute) t.GetCustomAttributes(typeof(DynamicEntityDtoAttribute)).Single())).ToList();
                var entityManager = scope.Resolve<DynamicEntityManager>();
                var entityInfos = entityManager.GetDynamicEntityInfos();
            
                foreach (var entityInfo in entityInfos)
                {
                    try
                    {
                        var entityDto = entityDtos.Single(e => e.attribute.TargetTypes.Any(t => t == entityInfo.EntityType));
                        RegisterEntityTypeForAppService(IocManager, entityInfo, entityDto.dto, AppServiceTypes.Default);
                    }
                    catch (Exception e)
                    {
                        RegisterEntityTypeForAppService(IocManager, entityInfo, entityManager.DynamicDtoType, AppServiceTypes.Default);
                    }
                }
            }

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }

        public void RegisterEntityTypeForAppService(
            IIocManager iocManager,
            DynamicEntityInfo entityInfo,
            Type entityDto,
            AutoAppServiceTypesAttribute autoAppServiceTypesAttribute)
        {

            RegisterEntityTypeForDbContext(
                iocManager,
                entityInfo,
                entityDto,
                autoAppServiceTypesAttribute.AppServiceTypesImplementation,
                autoAppServiceTypesAttribute.AppServiceTypesImplementationWithPrimaryKey
            );
        }

        public void RegisterEntityTypeForDbContext(IIocManager iocManager,
            DynamicEntityInfo entityInfo,
            Type entityDto,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityInfo.EntityType);
            if (primaryKeyType == typeof(int))
            {
                var implType =  repositoryImplementation.MakeGenericType(entityInfo.EntityType, entityDto);
                if (!iocManager.IsRegistered(implType))
                {
                    iocManager.IocContainer.Register(
                        Component
                            .For(implType)
                            .ImplementedBy(implType)
                            .Named(Guid.NewGuid().ToString("N"))
                            .LifestyleTransient()
                    );
                }
            }

            var genericImplType = repositoryImplementationWithPrimaryKey.MakeGenericType(entityInfo.EntityType, entityDto, primaryKeyType);
            if (!iocManager.IsRegistered(genericImplType))
            {
                iocManager.IocContainer.Register(
                    Component
                        .For(genericImplType)
                        .ImplementedBy(genericImplType)
                        .Named(Guid.NewGuid().ToString("N"))
                        .LifestyleTransient()
                );
            }
        }
    }
}

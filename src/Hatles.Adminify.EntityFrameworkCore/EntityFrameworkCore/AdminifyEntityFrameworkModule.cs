using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Configuration;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Hatles.Adminify.EntityFrameworkCore.Seed;
using Castle.MicroKernel.Registration;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.EntityFrameworkCore.DynamicEntities;

namespace Hatles.Adminify.EntityFrameworkCore
{
    [DependsOn(
        typeof(AdminifyCoreModule),
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class AdminifyEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<AdminifyDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        AdminifyDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        AdminifyDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
                
                Configuration.Modules.AbpEfCore().AddDbContext<AdminifyDynamicDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        AdminifyDynamicDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        AdminifyDynamicDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminifyEntityFrameworkModule).GetAssembly());
            
            using (IScopedIocResolver scope = IocManager.CreateScope())
            {
                Logger.Debug("Registering dynamic DbContext: " + typeof(AdminifyDynamicDbContext).AssemblyQualifiedName);

                var entityInfos = scope.Resolve<DynamicEntityManager>().GetDynamicEntityInfos();
            
                foreach (var entityInfo in entityInfos)
                {
                    RegisterEntityTypeForDbContext(typeof(AdminifyDynamicDbContext), IocManager, entityInfo, EfCoreAutoRepositoryTypes.Default);
                }
            }
        }

        public void RegisterEntityTypeForDbContext(Type dbContextType, 
            IIocManager iocManager, 
            DynamicEntityInfo entityInfo,
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute)
        {
            var autoRepositoryAttr = dbContextType.GetTypeInfo().GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>() ?? defaultAutoRepositoryTypesAttribute;

            RegisterEntityTypeForDbContext(
                iocManager,
                new EntityTypeInfo(entityInfo.EntityType, dbContextType),
                autoRepositoryAttr.RepositoryInterface,
                autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey,
                autoRepositoryAttr.RepositoryImplementation,
                autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
            );

            if (autoRepositoryAttr.WithDefaultRepositoryInterfaces)
            {
                RegisterEntityTypeForDbContext(
                    iocManager,
                    new EntityTypeInfo(entityInfo.EntityType, dbContextType),
                    defaultAutoRepositoryTypesAttribute.RepositoryInterface,
                    defaultAutoRepositoryTypesAttribute.RepositoryInterfaceWithPrimaryKey,
                    autoRepositoryAttr.RepositoryImplementation,
                    autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
                );
            }
        }

        public void RegisterEntityTypeForDbContext(IIocManager iocManager,
            EntityTypeInfo entityTypeInfo,
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
            if (primaryKeyType == typeof(int))
            {
                var genericRepositoryType = repositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                if (!iocManager.IsRegistered(genericRepositoryType))
                {
                    var implType = repositoryImplementation.GetGenericArguments().Length == 1
                        ? repositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                        : repositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType,
                            entityTypeInfo.EntityType);

                    iocManager.IocContainer.Register(
                        Component
                            .For(genericRepositoryType)
                            .ImplementedBy(implType)
                            .Named(Guid.NewGuid().ToString("N"))
                            .LifestyleTransient()
                    );
                }
            }

            var genericRepositoryTypeWithPrimaryKey =
                repositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType);
            if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
            {
                var implType = repositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                    ? repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType)
                    : repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.DeclaringType,
                        entityTypeInfo.EntityType, primaryKeyType);

                iocManager.IocContainer.Register(
                    Component
                        .For(genericRepositoryTypeWithPrimaryKey)
                        .ImplementedBy(implType)
                        .Named(Guid.NewGuid().ToString("N"))
                        .LifestyleTransient()
                );
            }
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
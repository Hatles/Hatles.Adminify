using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Hatles.Adminify.Authorization.Roles;
using Hatles.Adminify.Authorization.Users;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.MultiTenancy;

namespace Hatles.Adminify.EntityFrameworkCore.DynamicEntities
{
    public class AdminifyDynamicDbContext : AbpZeroCommonDbContext<Role, User, AdminifyDynamicDbContext>
    {
        private readonly DynamicEntityManager _dynamicEntityManager;

        public AdminifyDynamicDbContext(DbContextOptions<AdminifyDynamicDbContext> options, DynamicEntityManager dynamicEntityManager)
            : base(options)
        {
            _dynamicEntityManager = dynamicEntityManager;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var addMethod = typeof(AdminifyDynamicDbContext)
                .GetMethods()
                .Single(m => 
                    m.Name == nameof(AddConfiguration) 
                    && m.GetGenericArguments().Any(a => a.Name == "TEntity")
                    && m.GetGenericArguments().Any(a => a.Name == "TEntityConfiguration"));

            var entityInfos = _dynamicEntityManager.GetDynamicEntityInfos();
            
            foreach (var entityInfo in entityInfos)
            {
                if (entityInfo.EntityConfigurationType == null)
                {
                    AddEntity(modelBuilder, entityInfo.EntityType);
                }
                else
                {
                    AddConfiguration(modelBuilder, addMethod, entityInfo.EntityConfigurationType);
                }
            }

            AddEntity(modelBuilder, _dynamicEntityManager.DynamicType);

            base.OnModelCreating(modelBuilder);
        }
        
        public static ModelBuilder AddEntity(ModelBuilder modelBuilder, Type entityType)
        {
            modelBuilder.Entity(entityType);

            return modelBuilder;
        }
        
        public static ModelBuilder AddConfiguration(ModelBuilder modelBuilder, MethodInfo addMethod, Type entityType, Type configurationType)
        {
            addMethod.MakeGenericMethod(entityType, configurationType)
                .Invoke(null, new object[]{ modelBuilder });

            return modelBuilder;
        }
        
        public static ModelBuilder AddConfiguration(ModelBuilder modelBuilder, MethodInfo addMethod, Type configurationType)
        {
            var interfaceType = configurationType.GetInterface(typeof(IEntityTypeConfiguration<>).Name);
            var entityType = interfaceType.GetGenericArguments().Single();

            return AddConfiguration(modelBuilder, addMethod, entityType, configurationType);
        }
        
        public static ModelBuilder AddConfiguration<TEntity, TEntityConfiguration>(ModelBuilder modelBuilder)
        where TEntity : class
        where TEntityConfiguration : IEntityTypeConfiguration<TEntity>
        {
            var configuration = Activator.CreateInstance<TEntityConfiguration>();
            return modelBuilder.Entity<TEntity>(builder => configuration.Configure(builder));
        }
    }
}

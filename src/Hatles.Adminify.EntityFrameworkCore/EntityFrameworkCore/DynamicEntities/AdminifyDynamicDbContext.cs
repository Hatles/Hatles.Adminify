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
                AddConfiguration(modelBuilder, addMethod, entityInfo.EntityType);
            }

            base.OnModelCreating(modelBuilder);
        }
        
        public static ModelBuilder AddConfiguration(ModelBuilder modelBuilder, MethodInfo addMethod, Type configurationType)
        {
            var interfaceType = configurationType.GetInterface(typeof(IEntityTypeConfiguration<>).Name);
            var entityType = interfaceType.GetGenericArguments().Single();

            addMethod.MakeGenericMethod(entityType, configurationType)
                .Invoke(null, new object[]{ modelBuilder });

            return modelBuilder;
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

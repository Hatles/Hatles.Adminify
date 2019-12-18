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

namespace Hatles.Adminify.EntityFrameworkCore
{
    public class AdminifyDbContext : AbpZeroDbContext<Tenant, Role, User, AdminifyDbContext>
    {
        public AdminifyDbContext(DbContextOptions<AdminifyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

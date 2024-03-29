﻿using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using Hatles.Adminify.Authentication.JwtBearer;
using Hatles.Adminify.Configuration;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.EntityFrameworkCore;

namespace Hatles.Adminify
{
    [DependsOn(
        typeof(AdminifyApplicationModule),
        typeof(AdminifyEntityFrameworkModule),
        typeof(AbpAspNetCoreModule)
        , typeof(AbpAspNetCoreSignalRModule)
    )]
    public class AdminifyWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AdminifyWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AdminifyConsts.ConnectionStringName
            );

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AdminifyApplicationModule).GetAssembly()
                )
                .Where(type => !ReflectionHelper.IsAssignableToGenericType(type, typeof(IDynamicEntityService<,>)));

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AdminifyApplicationModule).GetAssembly(),
                    "dynamic"
                )
                .Where(type => ReflectionHelper.IsAssignableToGenericType(type, typeof(IDynamicEntityService<,>)))
                .ConfigureControllerModel(model =>
                {
                    var dynamicServicecType = model.ControllerType.GetInterfaces().Single(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDynamicEntityService<,>));
                    var entityType = dynamicServicecType.GetGenericArguments()[0];
                    
                    model.ControllerName = $"{entityType.Name}";
                    model.ApiExplorer.GroupName = $"Dynamic{entityType.Name}";
                });

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials =
                new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminifyWebCoreModule).GetAssembly());
        }
    }
}
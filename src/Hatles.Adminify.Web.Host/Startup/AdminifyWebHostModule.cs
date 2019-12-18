using Abp.AspNetCore.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Hatles.Adminify.Configuration;
using Hatles.Adminify.DynamicEntities;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Hatles.Adminify.Web.Host.Startup
{
    [DependsOn(
       typeof(AdminifyWebCoreModule))]
    public class AdminifyWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AdminifyWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminifyWebHostModule).GetAssembly());
            
            //Add feature providers
            var partManager = IocManager.Resolve<ApplicationPartManager>();
            var dynamicEntityManager = IocManager.Resolve<DynamicEntityManager>();
            partManager?.FeatureProviders.Add(new DynamicAppServiceControllerFeatureProvider(dynamicEntityManager));
        }
    }
}

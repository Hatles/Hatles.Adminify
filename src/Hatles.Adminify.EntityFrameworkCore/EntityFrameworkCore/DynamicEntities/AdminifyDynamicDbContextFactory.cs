using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Hatles.Adminify.Configuration;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.Web;

namespace Hatles.Adminify.EntityFrameworkCore.DynamicEntities
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AdminifyDynamicDbContextFactory : IDesignTimeDbContextFactory<AdminifyDynamicDbContext>
    {
        public AdminifyDynamicDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AdminifyDynamicDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AdminifyDynamicDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AdminifyConsts.ConnectionStringName));
            
            var dynamicEntityManager = new DynamicEntityManager();

            return new AdminifyDynamicDbContext(builder.Options, dynamicEntityManager);
        }
    }
}

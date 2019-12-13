using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Hatles.Adminify.Configuration;
using Hatles.Adminify.DynamicEntities;
using Hatles.Adminify.Web;

namespace Hatles.Adminify.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AdminifyDbContextFactory : IDesignTimeDbContextFactory<AdminifyDbContext>
    {
        public AdminifyDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AdminifyDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AdminifyDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AdminifyConsts.ConnectionStringName));

            return new AdminifyDbContext(builder.Options);
        }
    }
}

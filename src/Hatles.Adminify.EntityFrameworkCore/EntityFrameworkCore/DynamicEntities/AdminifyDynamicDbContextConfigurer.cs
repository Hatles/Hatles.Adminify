using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Hatles.Adminify.EntityFrameworkCore.DynamicEntities
{
    public static class AdminifyDynamicDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AdminifyDynamicDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AdminifyDynamicDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}

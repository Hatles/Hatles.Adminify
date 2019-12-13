using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Hatles.Adminify.EntityFrameworkCore
{
    public static class AdminifyDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AdminifyDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AdminifyDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}

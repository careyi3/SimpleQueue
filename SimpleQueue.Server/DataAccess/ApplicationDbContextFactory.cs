using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SimpleQueue.Common.DataAccess;

namespace SimpleQueue.Server.DataAccess
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private string _connectionString;
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(ConnectionString, b => b.MigrationsAssembly("SimpleQueue.Server"));

            var context = new ApplicationDbContext(optionsBuilder.Options);
            return context;
        }

        private string ConnectionString
        {
            get
            {
                if (_connectionString != null)
                {
                    return _connectionString;
                }
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile("appsettings.Development.json", false, true)
                    .Build();
                _connectionString = config.GetConnectionString("DefaultConnection");
                return _connectionString;
            }
        }
    }
}
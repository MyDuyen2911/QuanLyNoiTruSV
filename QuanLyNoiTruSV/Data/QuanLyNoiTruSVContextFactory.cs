using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QuanLyNoiTruSV.Data
{
    public class QuanLyNoiTruSVContextFactory : IDesignTimeDbContextFactory<QuanLyNoiTruSVContext>
    {
        public QuanLyNoiTruSVContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<QuanLyNoiTruSVContext>();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new QuanLyNoiTruSVContext(optionsBuilder.Options);
        }
    }
}
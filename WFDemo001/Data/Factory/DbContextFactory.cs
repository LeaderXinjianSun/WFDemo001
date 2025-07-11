using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFDemo001.Data.Factory
{
    public class DbContextFactory : IDbContextFactory
    {
        //private readonly IServiceProvider _serviceProvider;

        //public DbContextFactory(IServiceProvider serviceProvider)
        //{
        //    _serviceProvider = serviceProvider;
        //}

        public MDbContext Create()
        {
            return new MDbContext(CreateOptions());
        }

        private DbContextOptions<MDbContext> CreateOptions()
        {
            string connectString = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "point.db"
            );

            return new DbContextOptionsBuilder<MDbContext>()
                .UseSqlite($"Data Source={connectString}")
                .Options;
        }
    }
}

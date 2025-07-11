using Microsoft.EntityFrameworkCore;
using WFDemo001.Data.Models;

namespace WFDemo001.Data
{
    public class MDbContext : DbContext
    {
        public DbSet<Param> Params { get; set; }
        public MDbContext(DbContextOptions<MDbContext> options) : base(options)
        {
            //这个构造必须要有，它执行DbContext的注入
        }
    }
}

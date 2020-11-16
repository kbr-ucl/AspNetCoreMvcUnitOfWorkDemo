using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMvcUnitOfWorkDemo.Mvc.Data
{
    public class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext(DbContextOptions<MyDatabaseContext>
            options)
            : base(options)
        {
        }
    }
}
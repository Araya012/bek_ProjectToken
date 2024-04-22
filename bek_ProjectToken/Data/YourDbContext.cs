using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace bek_ProjectToken.Models
{
    public class YourDbContext : DbContext
    {
        public DbSet<Client> bek_Client { get; set; }
        public DbSet<Session> bek_Session { get; set; }
        public DbSet<User> bek_User { get; set; }

        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Here you can make additional configurations of your model if necessary
            //For example, establishing primary keys, relationships, indexes, etc.
        }
    }
}

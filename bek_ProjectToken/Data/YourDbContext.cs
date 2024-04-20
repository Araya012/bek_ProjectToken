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
            // Aquí puedes realizar configuraciones adicionales de tu modelo si es necesario
            // Por ejemplo, establecer claves primarias, relaciones, índices, etc.
        }
    }
}

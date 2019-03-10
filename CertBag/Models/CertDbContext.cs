using Microsoft.EntityFrameworkCore;

namespace CertBag.Models
{
    public class CertDbContext : DbContext
    {
        public CertDbContext(DbContextOptions<CertDbContext> options)
        : base(options)
        {

        }

        public DbSet<Certificate> Certificate { get; set; }
    }
}
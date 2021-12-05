using Microsoft.EntityFrameworkCore;

namespace CryptolioAPI.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PortfolioRecord> PortfolioRecords { get; set; }
        public DbSet<PriceRecord> PriceRecords { get; set; }
        public DbSet<Coin> Coins { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
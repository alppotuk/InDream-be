using Microsoft.EntityFrameworkCore;

namespace InDream.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<TrackedItem> TrackedItems { get; set; }
        public DbSet<SupportedSite> SupportedSites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TrackedItem>()
                .HasOne(ti => ti.Account) 
                .WithMany(a => a.TrackedItems)
                .HasForeignKey(ti => ti.AccountId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}

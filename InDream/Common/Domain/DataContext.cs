using InDream.Api.Features.Authentication.Data;
using InDream.Api.Features.Resource.Data;
using InDream.Api.Features.Tracking.Data;
using Microsoft.EntityFrameworkCore;

namespace InDream.Api.Common.Data;
public class DataContext : DbContext
{

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<TrackedItem> TrackedItems { get; set; }

    public DbSet<ResourceLanguage> ResourceLanguages { get; set; }
    public DbSet<ResourceString> ResourceStrings { get; set; }
    public DbSet<ResourceStringText> ResourceStringTexts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("dbo");


        modelBuilder.Entity<TrackedItem>()
            .HasOne(ti => ti.Account) 
            .WithMany(a => a.TrackedItems)
            .HasForeignKey(ti => ti.AccountId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}

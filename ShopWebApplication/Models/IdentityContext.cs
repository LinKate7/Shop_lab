using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopWebApplication.ViewModels;

namespace ShopWebApplication.Models
{
	public class IdentityContext : IdentityDbContext<UserForIdentity, IdentityRole<int>, int>
	{
		public IdentityContext(DbContextOptions<IdentityContext> options)
		: base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegisterViewModel>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
	    public DbSet<ShopWebApplication.ViewModels.RegisterViewModel> RegisterViewModel { get; set; } = default!;

    }
}


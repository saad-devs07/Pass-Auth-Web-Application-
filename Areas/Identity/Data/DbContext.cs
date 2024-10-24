using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassAuthWebApp_.Areas.Identity.Data;

namespace PassAuthWebApp_.Areas.Identity.Data;

public class DbContext : IdentityDbContext<ApplicationUser>
{
    public DbContext(DbContextOptions<DbContext> options) : base(options) {}

    public DbSet<PasswordHistory> PasswordHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
               .Property(u => u.LastPasswordChangedDate)
               .IsRequired();

        builder.Entity<ApplicationUser>()
               .Property(u => u.IsDormant)
               .IsRequired();
    }
}

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpotifAi.Users;

namespace SpotifAi.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) :
    IdentityDbContext<
        User,
        UserRole,
        Guid
    >(options), IDataProtectionKeyContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("SpotifAi");

        builder.Entity<User>(ConfigureUser);
    }

    private static void ConfigureUser(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
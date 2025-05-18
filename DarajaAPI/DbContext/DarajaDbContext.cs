using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DarajaAPI.Models.Domain;

namespace Daraja.DbContext
{
    public class DarajaDbContext : IdentityDbContext<ApplicationUser>
    {
        public DarajaDbContext(DbContextOptions<DarajaDbContext> options)
            : base(options)
        {
        }

        public DbSet<DarajaSetting> DarajaSettings { get; set; }
        public DbSet<MailRequest> MailRequests { get; set; }
        public DbSet<MailSetting> MailSettings { get; set; }
        public DbSet<MpesaC2B> MpesaC2Bs { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<WelcomeRequest> WelcomeRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }
    }
}

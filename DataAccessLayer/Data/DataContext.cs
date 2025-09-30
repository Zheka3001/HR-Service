using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DataAccessLayer.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Role)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(u => u.RefreshToken).IsRequired(false);
                entity.Property(u => u.RefreshTokenExpiryTime).IsRequired(false);
                entity.Property(u => u.MiddleName).IsRequired(false);

                entity.HasIndex(u => u.Login)
                    .IsUnique();
            });

            SeedAdminUser(modelBuilder);
        }

        private void SeedAdminUser(ModelBuilder modelBuilder)
        {
            using var hmac = new HMACSHA512();

            var adminUser = new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "Default",
                MiddleName = null,
                Login = "admin",
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key
            };

            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}

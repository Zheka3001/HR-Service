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
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<WorkGroup> WorkGroups { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<ApplicantInfo> ApplicantInfos { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Role)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(u => u.MiddleName).IsRequired(false);

                entity.HasIndex(u => u.Login)
                    .IsUnique();
            });

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkGroup>()
                .HasMany(wg => wg.Users)
                .WithOne(u => u.WorkGroup)
                .HasForeignKey(u => u.WorkGroupId);

            modelBuilder.Entity<WorkGroup>()
                .HasMany(wg => wg.Applicants)
                .WithOne(a => a.WorkGroup)
                .HasForeignKey(a => a.WorkGroupId);

            modelBuilder.Entity<Applicant>()
                .HasOne(a => a.ApplicantInfo)
                .WithMany()
                .HasForeignKey(a => a.ApplicantInfoId);

            modelBuilder.Entity<Applicant>()
                .Property(a => a.WorkSchedule)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicantInfo>()
                .HasMany(ai => ai.SocialNetworks)
                .WithOne(sn => sn.ApplicantInfo)
                .HasForeignKey(sn => sn.ApplicantInfoId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ApplicantInfo)
                .WithMany()
                .HasForeignKey(e => e.ApplicantInfoId);

            modelBuilder.Entity<SocialNetwork>()
                .Property(sn => sn.Type)
                .HasConversion<string>();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                modelBuilder.Entity(entityType.ClrType).ToTable(ConvertToSnakeCase(tableName));
            }
        }

        private string ConvertToSnakeCase(string tableName)
        {
            return string.Concat(tableName.Select((ch, i) => 
                (i > 0 && char.IsUpper(ch) ? "_" : "") + char.ToLower(ch)));
        }
    }
}

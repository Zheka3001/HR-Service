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

        public DbSet<UserDao> Users { get; set; }
        public DbSet<RefreshTokenDao> RefreshTokens { get; set; }
        public DbSet<WorkGroupDao> WorkGroups { get; set; }
        public DbSet<ApplicantDao> Applicants { get; set; }
        public DbSet<ApplicantInfoDao> ApplicantInfos { get; set; }
        public DbSet<EmployeeDao> Employees { get; set; }
        public DbSet<SocialNetworkDao> SocialNetworks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDao>(entity =>
            {
                entity.Property(u => u.Role)
                    .HasConversion<string>()
                    .IsRequired();

                entity.Property(u => u.MiddleName).IsRequired(false);

                entity.HasIndex(u => u.Login)
                    .IsUnique();

                entity.HasMany(u => u.CreatedApplicants)
                    .WithOne(a => a.CreatedBy)
                    .HasForeignKey(a => a.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RefreshTokenDao>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkGroupDao>()
                .HasMany(wg => wg.Users)
                .WithOne(u => u.WorkGroup)
                .HasForeignKey(u => u.WorkGroupId);

            modelBuilder.Entity<WorkGroupDao>()
                .HasMany(wg => wg.Applicants)
                .WithOne(a => a.WorkGroup)
                .HasForeignKey(a => a.WorkGroupId);

            modelBuilder.Entity<ApplicantDao>()
                .HasOne(a => a.ApplicantInfo)
                .WithMany()
                .HasForeignKey(a => a.ApplicantInfoId);

            modelBuilder.Entity<ApplicantDao>()
                .Property(a => a.WorkSchedule)
                .HasConversion<string>();

            modelBuilder.Entity<ApplicantDao>()
                .HasOne(a => a.LastUpdatedBy)
                .WithMany()
                .HasForeignKey(a => a.LastUpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicantInfoDao>()
                .HasMany(ai => ai.SocialNetworks)
                .WithOne(sn => sn.ApplicantInfo)
                .HasForeignKey(sn => sn.ApplicantInfoId);

            modelBuilder.Entity<EmployeeDao>()
                .HasOne(e => e.ApplicantInfo)
                .WithMany()
                .HasForeignKey(e => e.ApplicantInfoId);

            modelBuilder.Entity<SocialNetworkDao>()
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

using Backend.Domain.Entities;
using Backend.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 1. DbSet MUST be at the class level
        public DbSet<CandidateProfile> CandidateProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply separate configuration classes (if you have any in the Configurations folder)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // 2. Configure CandidateProfile relationship and index using 'modelBuilder'
            modelBuilder.Entity<CandidateProfile>(entity =>
            {
                // Link to ApplicationUser (One-to-One)
                entity.HasOne<ApplicationUser>()
                      .WithOne()
                      .HasForeignKey<CandidateProfile>(cp => cp.UserId)
                      .IsRequired();

                // Ensure one profile per user
                entity.HasIndex(cp => cp.UserId).IsUnique();
            });
        }
    }
}
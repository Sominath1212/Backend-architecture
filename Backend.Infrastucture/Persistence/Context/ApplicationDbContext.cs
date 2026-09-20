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
        public DbSet<Education> EducationRecords { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Resume> Resumes { get; set; }

        public DbSet<Skill> Skills { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<RecruiterProfile> RecruiterProfiles { get; set; }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }

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

            modelBuilder.Entity<Education>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                      .WithMany() // A user can have many education records
                      .HasForeignKey(e => e.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); // Delete education records if user is deleted

                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<Experience>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasIndex(s => s.Name).IsUnique();
            });

            modelBuilder.Entity<CandidateSkill>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(cs => cs.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cs => cs.Skill)
                      .WithMany()
                      .HasForeignKey(cs => cs.SkillId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a master skill if candidates have it

                entity.HasIndex(cs => new { cs.UserId, cs.SkillId }).IsUnique(); // One skill per candidate
            });

            modelBuilder.Entity<Resume>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => r.UserId);
            });

            // 1. Add these DbSets at the class level

            // 2. Add these configurations inside OnModelCreating:
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
            });

            modelBuilder.Entity<RecruiterProfile>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Company)
                      .WithMany(c => c.Recruiters)
                      .HasForeignKey(r => r.CompanyId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => new { r.UserId, r.CompanyId }).IsUnique();
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasOne(j => j.Company)
                      .WithMany()
                      .HasForeignKey(j => j.CompanyId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(j => j.PostedByUserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(j => j.Status);
                entity.HasIndex(j => j.CompanyId);
                entity.HasIndex(j => j.CreatedAt);
            });

            modelBuilder.Entity<JobSkill>(entity =>
            {
                entity.HasOne(js => js.Job)
                      .WithMany(j => j.JobSkills)
                      .HasForeignKey(js => js.JobId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(js => js.Skill)
                      .WithMany()
                      .HasForeignKey(js => js.SkillId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(js => new { js.JobId, js.SkillId }).IsUnique();
            });
        }
    }
}
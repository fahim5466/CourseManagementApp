using Application;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpHelper httpHelper) : DbContext(options), IUnitOfWork
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseClass> CourseClasses { get; set; }
        public virtual DbSet<ClassEnrollment> ClassEnrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            SeedData(modelBuilder);
        }

        public void SeedData(ModelBuilder modelBuilder)
        {
            // Add roles.

            Role adminRole = new() { Id = new Guid("519BC5A9-8BC3-495B-A320-4AFAEE4CD363"), Name = Role.ADMIN };
            Role staffRole = new() { Id = new Guid("214E0C6B-E213-40D1-A1F2-169B0026F882"), Name = Role.STAFF };
            Role studentRole = new() { Id = new Guid("DCC31107-75B5-4504-8E87-0D654323C56D"), Name = Role.STUDENT };

            modelBuilder.Entity<Role>().HasData(adminRole);
            modelBuilder.Entity<Role>().HasData(staffRole);
            modelBuilder.Entity<Role>().HasData(studentRole);

            // Add admin user.

            User adminUser = new()
            {
                Id = new Guid("B8C35758-1881-47AD-8994-0529DF7C70ED"),
                Name = "Admin",
                Email = "admin@test.com",
                PasswordHash = "$2a$13$IiHfZUPeaA5zu22M57YZB.riJVHfuTwMpw4z9sVvpvdqstDRtt21O",
                IsEmailVerified = true,
                EmailVerificationToken = null,
                EmailVerificationTokenExpires = null,
                RefreshTokenHash = null,
                RefreshTokenExpires = null
            };

            modelBuilder.Entity<User>().HasData(adminUser);

            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });

            // Add staff user.

            User staffUser = new()
            {
                Id = new Guid("EE13B6C6-A98E-440B-93F8-ECF083D51052"),
                Name = "Staff",
                Email = "staff@test.com",
                PasswordHash = "$2a$13$IiHfZUPeaA5zu22M57YZB.riJVHfuTwMpw4z9sVvpvdqstDRtt21O",
                IsEmailVerified = true,
                EmailVerificationToken = null,
                EmailVerificationTokenExpires = null,
                RefreshTokenHash = null,
                RefreshTokenExpires = null
            };

            modelBuilder.Entity<User>().HasData(staffUser);

            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = staffUser.Id, RoleId = staffRole.Id });

            // Add student user.

            User studentUser = new()
            {
                Id = new Guid("2E23583F-D118-4295-8A80-09266B7497DB"),
                Name = "Student",
                Email = "student@test.com",
                PasswordHash = "$2a$13$IiHfZUPeaA5zu22M57YZB.riJVHfuTwMpw4z9sVvpvdqstDRtt21O",
                IsEmailVerified = true,
                EmailVerificationToken = null,
                EmailVerificationTokenExpires = null,
                RefreshTokenHash = null,
                RefreshTokenExpires = null
            };

            modelBuilder.Entity<User>().HasData(studentUser);

            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = studentUser.Id, RoleId = studentRole.Id });
        }

        public override int SaveChanges()
        {
            SetAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditFields()
        {
            IEnumerable<EntityEntry> entries = ChangeTracker.Entries()
                                                            .Where(e => e.Entity is IAuditable && e.State == EntityState.Added);

            foreach (EntityEntry entry in entries)
            {
                IAuditable auditable = (IAuditable)entry.Entity;
                auditable.CreatedBy = httpHelper.GetCurrentUserId();
                auditable.CreatedOn = DateTime.UtcNow;
            }
        }

    }
}

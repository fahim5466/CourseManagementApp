using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Relationships;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            User adminUser = new User
            {
                Id = new Guid("B8C35758-1881-47AD-8994-0529DF7C70ED"),
                Name = "Admin",
                Email = "admin@test.com",
                PasswordHash = "$2a$13$GFEm2Ijr3Jd4GsbRXp8Yn.YVrCAoatoFjfm458GFMpd5UFzyShqCq",
                IsEmailVerified = true,
                EmailVerificationTokenHash = null,
                EmailVerificationTokenHashExpires = null,
                RefreshTokenHash = null,
                RefreshTokenExpires = null
            };

            // Add admin user.
            modelBuilder.Entity<User>().HasData(adminUser);

            Role adminRole = new Role { Id = new Guid("519BC5A9-8BC3-495B-A320-4AFAEE4CD363"), Name = Role.ADMIN };
            Role staffRole = new Role { Id = new Guid("214E0C6B-E213-40D1-A1F2-169B0026F882"), Name = Role.STAFF };
            Role studentRole = new Role { Id = new Guid("DCC31107-75B5-4504-8E87-0D654323C56D"), Name = Role.STUDENT };

            // Add roles
            modelBuilder.Entity<Role>().HasData(adminRole);
            modelBuilder.Entity<Role>().HasData(staffRole);
            modelBuilder.Entity<Role>().HasData(studentRole);

            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
        }
    }
}

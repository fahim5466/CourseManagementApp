﻿using Domain.Entities.Roles;
using Domain.Entities.Users;
using Domain.Relationships;
using Application;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
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

            SeedData(modelBuilder);
        }

        public void SeedData(ModelBuilder modelBuilder)
        {
            // Add roles.

            Role adminRole = new Role { Id = new Guid("519BC5A9-8BC3-495B-A320-4AFAEE4CD363"), Name = Role.ADMIN };
            Role staffRole = new Role { Id = new Guid("214E0C6B-E213-40D1-A1F2-169B0026F882"), Name = Role.STAFF };
            Role studentRole = new Role { Id = new Guid("DCC31107-75B5-4504-8E87-0D654323C56D"), Name = Role.STUDENT };

            modelBuilder.Entity<Role>().HasData(adminRole);
            modelBuilder.Entity<Role>().HasData(staffRole);
            modelBuilder.Entity<Role>().HasData(studentRole);

            // Add admin user.

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

            modelBuilder.Entity<User>().HasData(adminUser);

            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });

            // Add staff user.

            User staffUser = new User
            {
                Id = new Guid("EE13B6C6-A98E-440B-93F8-ECF083D51052"),
                Name = "Staff",
                Email = "staff@test.com",
                PasswordHash = "$2a$13$902uxeLjnX9nd.78KtdZNOUQNrzoCGvdug1dUiV33NRNNblqe7VA6",
                IsEmailVerified = true,
                EmailVerificationTokenHash = null,
                EmailVerificationTokenHashExpires = null,
                RefreshTokenHash = null,
                RefreshTokenExpires = null
            };

            modelBuilder.Entity<User>().HasData(staffUser);

            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = staffUser.Id, RoleId = staffRole.Id });

            // Add student user.

            User studentUser = new User
            {
                Id = new Guid("2E23583F-D118-4295-8A80-09266B7497DB"),
                Name = "Student",
                Email = "student@test.com",
                PasswordHash = "$2a$13$7ZX.H.wI0yitiqiHnE9D8Om.T6F0boCdYS5EmKviXkl7m8uwefY4e",
                IsEmailVerified = true,
                EmailVerificationTokenHash = null,
                EmailVerificationTokenHashExpires = null,
                RefreshTokenHash = null,
                RefreshTokenExpires = null
            };

            modelBuilder.Entity<User>().HasData(studentUser);

            modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = studentUser.Id, RoleId = studentRole.Id });
        }

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync();
        }
    }
}

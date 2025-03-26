using Domain.Entities;
using Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasMaxLength(Role.NAME_MAX_LENGTH);

            builder.HasMany(x => x.Users)
                   .WithMany(x => x.Roles)
                   .UsingEntity<UserRole>();
        }
    }
}

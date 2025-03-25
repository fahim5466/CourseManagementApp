using Domain.Entities.Users;
using Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Name)
                   .HasMaxLength(User.NAME_MAX_LENGTH);

            builder.Property(x => x.Email)
                   .HasMaxLength(User.EMAIL_MAX_LENGTH);

            builder.Property(x => x.PasswordHash)
                   .HasMaxLength(User.HASH_MAX_LENGTH);

            builder.Property(x => x.EmailVerificationToken)
                   .HasMaxLength(User.HASH_MAX_LENGTH);

            builder.Property(x => x.RefreshTokenHash)
                   .HasMaxLength(User.HASH_MAX_LENGTH);

            builder.HasMany(x => x.Roles)
                   .WithMany(x => x.Users)
                   .UsingEntity<UserRole>();
        }
    }
}

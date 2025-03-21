using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasMaxLength(User.NAME_MAX_LENGTH);

            builder.Property(x => x.Email)
                   .HasMaxLength(User.EMAIL_MAX_LENGTH);

            builder.Property(x => x.PasswordHash)
                   .HasMaxLength(User.HASH_MAX_LENGTH);

            builder.Property(x => x.EmailVerificationTokenHash)
                   .HasMaxLength(User.HASH_MAX_LENGTH);

            builder.Property(x => x.RefreshTokenHash)
                   .HasMaxLength(User.HASH_MAX_LENGTH);
        }
    }
}

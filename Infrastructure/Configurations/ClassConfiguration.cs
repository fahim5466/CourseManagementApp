using Domain.Entities;
using Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Name).HasMaxLength(Class.NAME_MAX_LENGTH);

            builder.HasMany(x => x.Courses)
                   .WithMany(x => x.Classes)
                   .UsingEntity<CourseClass>(
                        l => l.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId),
                        r => r.HasOne(x => x.Class).WithMany().HasForeignKey(x => x.ClassId)
                    );
        }
    }
}

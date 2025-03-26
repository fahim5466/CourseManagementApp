using Domain.Entities;
using Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Name).HasMaxLength(Class.NAME_MAX_LENGTH);

            builder.HasMany(x => x.Classes)
                   .WithMany(x => x.Courses)
                   .UsingEntity<CourseClass>(
                        l => l.HasOne(x => x.Class).WithMany().HasForeignKey(x => x.ClassId),
                        r => r.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId)
                        
                    ); ;
        }
    }
}

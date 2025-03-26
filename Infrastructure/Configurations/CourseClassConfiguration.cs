using Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CourseClassConfiguration : IEntityTypeConfiguration<CourseClass>
    {
        public void Configure(EntityTypeBuilder<CourseClass> builder)
        {
            builder.HasKey(x => new { x.CourseId, x.ClassId });

            builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Class).WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

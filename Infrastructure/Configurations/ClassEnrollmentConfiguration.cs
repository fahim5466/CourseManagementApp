using Domain.Entities;
using Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ClassEnrollmentConfiguration : IEntityTypeConfiguration<ClassEnrollment>
    {
        public void Configure(EntityTypeBuilder<ClassEnrollment> builder)
        {
            builder.HasKey(x => new { x.ClassId, x.StudentId });

            builder.HasOne(x => x.Class).WithMany().HasForeignKey(x => x.ClassId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

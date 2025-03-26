using Domain.Entities;

namespace Domain.Relationships
{
    public class CourseClass
    {
        #region Properties
        public Guid CourseId { get; set; }
        public Guid ClassId { get; set; }
        #endregion

        #region Navigation properties
        public Course Course { get; set; } = null!;
        public Class Class { get; set; } = null!;
        #endregion
    }
}

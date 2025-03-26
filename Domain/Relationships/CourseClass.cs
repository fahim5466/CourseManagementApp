using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Relationships
{
    public class CourseClass
    {
        #region Properties
        [Key, Column(Order = 0)]
        public Guid CourseId { get; set; }
        [Key, Column(Order = 1)]
        public Guid ClassId { get; set; }
        #endregion

        #region Navigation properties
        public Course Course { get; set; } = null!;
        public Class Class { get; set; } = null!;
        #endregion
    }
}

namespace Application.DTOs.Enrollment
{
    public class CourseEnrollmentInfoDto
    {
        public Guid CourseId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

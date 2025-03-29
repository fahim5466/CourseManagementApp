namespace Application.DTOs.Course
{
    public class CourseEnrollmentRequestDto
    {
        public required string CourseId { get; set; }
        public required string StudentId { get; set; }
    }
}

namespace Application.DTOs.Enrollment
{
    public class EnrollmentInfoResponseDto
    {
        public bool IsEnrolled { get; set; }
        public ClassEnrollmentInfoDto? DirectEnrollment { get; set; }
        public List<CourseEnrollmentInfoDto> IndirectEnrollmentsThroughCourses { get; set; } = [];
    }
}

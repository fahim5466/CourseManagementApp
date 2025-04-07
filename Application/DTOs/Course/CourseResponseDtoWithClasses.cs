namespace Application.DTOs.Course
{
    public class CourseResponseDtoWithClasses : CourseResponseDto
    {
        public required List<string> ClassNames { get; set; }
    }
}

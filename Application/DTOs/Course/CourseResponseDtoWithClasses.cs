namespace Application.DTOs.Course
{
    public class CourseResponseDtoWithClasses : CourseResponseDto
    {
        public List<string> ClassNames { get; set; } = [];
    }
}

namespace Application.DTOs.Course
{
    public class CourseResponseDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public List<string> ClassNames { get; set; } = [];
    }
}

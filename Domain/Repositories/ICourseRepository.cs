using Domain.Entities;
using Domain.Relationships;

namespace Domain.Repositories
{
    public interface ICourseRepository
    {
        public Task<Course?> GetCourseByIdAsync(string id);
        public Task<Course?> GetCourseByIdWithClassesAsync(string id);
        public Task<Course?> GetCourseByNameAsync(string name);
        public Task<Course?> GetCourseByNameAsync(string name, string idToExclude);
        public Task<List<Course>> GetAllCoursesAsync();
        public Task<List<User>> GetStudentsOfCourseAsync(string id);
        public Task CreateCourseAsync(Course course);
        public Task DeleteCourseAsync(Course course);
        public Task<CourseEnrollment?> GetCourseEnrollmentAsync(Guid courseId, Guid studentId);
        public Task CreateCourseEnrollmentAsync(CourseEnrollment courseEnrollment);
    }
}

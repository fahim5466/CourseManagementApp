using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICourseRepository
    {
        public Task<Course?> GetCourseByIdWithClassesAsync(string id);
        public Task<Course?> GetCourseByNameAsync(string name);
        public Task<Course?> GetCourseByNameAsync(string name, string idToExclude);
        public Task<List<Course>> GetAllCoursesAsync();
        public Task CreateCourseAsync(Course course);
        public Task DeleteCourseAsync(Course course);
    }
}

using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICourseRepository
    {
        public Task<Course?> GetCourseByNameAsync(string name);
        public Task CreateCourseAsync(Course course);
        public Task<Course?> GetCourseByIdWithClassesAsync(string id);
    }
}

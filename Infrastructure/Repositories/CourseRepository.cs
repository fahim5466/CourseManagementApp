using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository(ApplicationDbContext dbContext) : ICourseRepository
    {
        public async Task<Course?> GetCourseByIdWithClassesAsync(string id)
        {
            if(!Guid.TryParse(id, out Guid guid))
            {
                return null;
            }

            return await dbContext.Courses
                                  .Include(x => x.Classes)
                                  .FirstOrDefaultAsync(c => c.Id == guid);
        }

        public async Task<Course?> GetCourseByNameAsync(string name)
        {
            return await dbContext.Courses.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task CreateCourseAsync(Course course)
        {
            dbContext.Courses.Add(course);
            await dbContext.SaveChangesAsync();
        }
    }
}

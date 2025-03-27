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

        public async Task<Course?> GetCourseByNameAsync(string name, string idToExclude)
        {
            Guid guid = Guid.Empty;
            Guid.TryParse(idToExclude, out guid);

            return await dbContext.Courses.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != guid);
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await dbContext.Courses.Include(c => c.Classes).ToListAsync();
        }

        public async Task CreateCourseAsync(Course course)
        {
            dbContext.Courses.Add(course);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(Course course)
        {
            dbContext.Courses.Remove(course);
            await dbContext.SaveChangesAsync();
        }
    }
}

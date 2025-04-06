using Domain.Entities;
using Domain.Relationships;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CourseRepository(ApplicationDbContext dbContext) : ICourseRepository
    {
        public async Task<Course?> GetCourseByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                return null;
            }

            return await dbContext.Courses.FirstOrDefaultAsync(c => c.Id == guid);
        }

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

        public async Task<List<User>> GetStudentsOfCourseAsync(string id)
        {
            IQueryable<User> query = from courseEnrollment in dbContext.CourseEnrollments
                                     join student in dbContext.Users
                                     on courseEnrollment.StudentId equals student.Id
                                     where courseEnrollment.CourseId.ToString() == id
                                     select student;
            
            return await query.ToListAsync();
        }

        public async Task<List<Course>> GetCoursesOfStudentAsync(string id)
        {
            IQueryable<Course> query = from courseEnrollment in dbContext.CourseEnrollments
                                       join course in dbContext.Courses
                                       on courseEnrollment.CourseId equals course.Id
                                       where courseEnrollment.StudentId.ToString() == id
                                       select course;

            return await query.ToListAsync();
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

        public async Task<CourseEnrollment?> GetCourseEnrollmentAsync(Guid courseId, Guid studentId)
        {
            return await dbContext.CourseEnrollments.FirstOrDefaultAsync(cr => cr.CourseId == courseId && cr.StudentId == studentId);
        }

        public async Task<List<CourseEnrollment>> GetCourseEnrollmentsByClassAndStudentAsync(Guid classId, Guid studentId)
        {
            IQueryable<CourseEnrollment> query = from courseEnrollment in dbContext.CourseEnrollments
                                                 join courseClass in dbContext.CourseClasses
                                                 on courseEnrollment.CourseId equals courseClass.CourseId
                                                 where courseEnrollment.StudentId == studentId &&
                                                       courseClass.ClassId == classId
                                                select courseEnrollment;

            return await query.ToListAsync();
        }

        public async Task CreateCourseEnrollmentAsync(CourseEnrollment courseEnrollment)
        {
            dbContext.CourseEnrollments.Add(courseEnrollment);
            await dbContext.SaveChangesAsync();
        }
    }
}

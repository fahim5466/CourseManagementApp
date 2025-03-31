using Domain.Entities;
using Domain.Relationships;
using Domain.Repositories;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClassRepository(ApplicationDbContext dbContext) : IClassRepository
    {
        public async Task<Class?> GetClassByNameAsync(string name)
        {
            return await dbContext.Classes.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<Class?> GetClassByNameAsync(string name, string idToExclude)
        {
            Guid guid = Guid.Empty;
            Guid.TryParse(idToExclude, out guid);

            return await dbContext.Classes.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != guid);
        }

        public async Task<Class?> GetClassByIdAsync(string id)
        {
            if(!Guid.TryParse(id, out Guid guid))
            {
                return null;
            }

            return await dbContext.Classes.FirstOrDefaultAsync(c => c.Id == guid);
        }

        public async Task<Class?> GetClassByIdWithCoursesAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                return null;
            }

            return await dbContext.Classes
                                  .Include(x => x.Courses)
                                  .FirstOrDefaultAsync(c => c.Id == guid);
        }

        public async Task<List<Class>> GetClassesByIdAsync(List<string> ids)
        {
            List<Class> classes = [];

            foreach(string id in ids)
            {
                Class? clss = await GetClassByIdAsync(id);

                if(clss is not null && !classes.Contains(clss))
                {
                    classes.Add(clss);
                }
            }

            return classes;
        }

        public async Task<List<Class>> GetAllClassesAsync()
        {
            return await dbContext.Classes.ToListAsync();
        }

        public async Task<List<User>> GetStudentsOfClassAsync(string id)
        {
            IQueryable<User> query1 = from classEnrollment in dbContext.ClassEnrollments
                                      join student in dbContext.Users
                                      on classEnrollment.StudentId equals student.Id
                                      where classEnrollment.ClassId.ToString() == id
                                      select student;

            IQueryable<User> query2 = from courseEnrollment in dbContext.CourseEnrollments
                                      join student in dbContext.Users
                                      on courseEnrollment.StudentId equals student.Id
                                      join courseClass in dbContext.CourseClasses
                                      on courseEnrollment.CourseId equals courseClass.CourseId
                                      where courseClass.ClassId.ToString() == id
                                      select student;

            IQueryable<User> unionQuery = query1.Union(query2);

            return await unionQuery.ToListAsync();
        }

        public async Task<List<Class>> GetClassesOfStudentAsync(string id)
        {
            // Get classes where the student is directly enrolled.
            IQueryable<Class> query1 = from classEnrollment in dbContext.ClassEnrollments
                                       join clss in dbContext.Classes
                                       on classEnrollment.ClassId equals clss.Id
                                       where classEnrollment.StudentId.ToString() == id
                                       select clss;

            // Get classes where the student is indirectly enrolled through a course.
            IQueryable<Class> query2 = from courseEnrollment in dbContext.CourseEnrollments
                                       join courseClass in dbContext.CourseClasses
                                       on courseEnrollment.CourseId equals courseClass.CourseId
                                       join clss in dbContext.Classes
                                       on courseClass.ClassId equals clss.Id
                                       where courseEnrollment.StudentId.ToString() == id
                                       select clss;

            IQueryable<Class> unionQuery = query1.Union(query2);

            return await unionQuery.ToListAsync();
        }

        public async Task<bool> AreClassIdsValidAsync(List<string> ids)
        {
            foreach (string id in ids)
            {
                Class? clss = await GetClassByIdAsync(id);

                if (clss is null)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task CreateClassAsync(Class clss)
        {
            dbContext.Classes.Add(clss);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteClassAsync(Class clss)
        {
            dbContext.Classes.Remove(clss);
            await dbContext.SaveChangesAsync();
        }

        public async Task<ClassEnrollment?> GetClassEnrollmentAsync(Guid classId, Guid studentId)
        {
            return await dbContext.ClassEnrollments.FirstOrDefaultAsync(cr => cr.ClassId == classId && cr.StudentId == studentId);
        }

        public async Task CreateClassEnrollmentAsync(ClassEnrollment classEnrollment)
        {
            dbContext.ClassEnrollments.Add(classEnrollment);
            await dbContext.SaveChangesAsync();
        }
    }
}

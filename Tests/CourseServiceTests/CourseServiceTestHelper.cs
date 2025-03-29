using Application.Services;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Repositories;

namespace Tests.CourseServiceTests
{
    public class CourseServiceTestHelper
    {
        public static CourseService GetCourseService(ApplicationDbContext dbContext)
        {
            IClassRepository classRepository = new ClassRepository(dbContext);
            ICourseRepository courseRepository = new CourseRepository(dbContext);
            IUserRepository userRepository = new UserRepository(dbContext);

            return new(courseRepository, classRepository, userRepository, dbContext);
        }
    }
}

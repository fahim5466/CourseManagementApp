using Application.Services;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Repositories;

namespace Tests.ClassServiceTests
{
    public class ClassServiceTestHelper
    {
        public static ClassService GetClassService(ApplicationDbContext dbContext)
        {
            IClassRepository classRepository = new ClassRepository(dbContext);
            ICourseRepository courseRepository = new CourseRepository(dbContext);
            IUserRepository userRepository = new UserRepository(dbContext);

            return new(classRepository, courseRepository, userRepository, dbContext);
        }
    }
}

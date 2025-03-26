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

            return new(classRepository, dbContext);
        }
    }
}

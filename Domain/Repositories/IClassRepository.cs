using Domain.Entities;
using Domain.Relationships;

namespace Domain.Repositories
{
    public interface IClassRepository
    {
        public Task<Class?> GetClassByNameAsync(string name);
        public Task<Class?> GetClassByNameAsync(string name, string idToExclude);
        public Task<Class?> GetClassByIdAsync(string id);
        public Task<List<Class>> GetClassesByIdAsync(List<string> ids);
        public Task<List<Class>> GetAllClassesAsync();
        public Task<bool> AreClassIdsValidAsync(List<string> ids);
        public Task CreateClassAsync(Class clss);
        public Task DeleteClassAsync(Class clss);
        public Task<ClassEnrollment?> GetClassEnrollmentAsync(Guid classId, Guid studentId);
        public Task CreateClassEnrollmentAsync(ClassEnrollment classEnrollment);
        public Task<Class?> GetClassByIdWithCoursesAsync(string id);
    }
}

using Domain.Entities;

namespace Domain.Repositories
{
    public interface IClassRepository
    {
        public Task<Class?> GetClassByNameAsync(string name);
        public Task<Class?> GetClassByNameAsync(string name, string idToExclude);
        public Task<Class?> GetClassByIdAsync(string id);
        public Task<List<Class>> GetAllClassesAsync();
        public Task CreateClassAsync(Class clss);
        public Task DeleteClassAsync(Class clss);
    }
}

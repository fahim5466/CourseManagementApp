using Domain.Entities;

namespace Domain.Repositories
{
    public interface IClassRepository
    {
        public Task<Class?> GetClassByNameAsync(string name);
        public Task CreateClassAsync(Class clss);
    }
}

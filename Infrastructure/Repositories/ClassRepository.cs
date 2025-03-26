using Domain.Entities;
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

        public async Task CreateClassAsync(Class clss)
        {
            dbContext.Classes.Add(clss);
            await dbContext.SaveChangesAsync();
        }
    }
}

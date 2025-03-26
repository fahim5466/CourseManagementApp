﻿using Domain.Entities;
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

        public async Task<List<Class>> GetAllClassesAsync()
        {
            return await dbContext.Classes.ToListAsync();
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
    }
}

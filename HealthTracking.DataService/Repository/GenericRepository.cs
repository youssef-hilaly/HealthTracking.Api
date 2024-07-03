using HealthTracking.DataService.Data;
using HealthTracking.DataService.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracking.DataService.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // protected to use it in the subclasses
        protected readonly AppDbContext _context;

        internal DbSet<T> dbSet;

        protected readonly ILogger _logger;

        public GenericRepository(AppDbContext context, ILogger logger)
        {
            // check if we use it 
            _context = context;
            dbSet = context.Set<T>();
            _logger = logger;
        }
        public virtual async Task<bool> Add(T entity)
        { 
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual Task<bool> Delete(Guid id, string userId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
    }
}

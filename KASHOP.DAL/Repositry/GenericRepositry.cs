using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repositry
{
    public class GenericRepositry<T> : IGenericRepositry<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public GenericRepositry(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string[]? includes = null)
        {

            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
                query = query.Where(filter);

            if(includes != null)
            {
                foreach(var include in includes)
                {
                   query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetOne(Expression<Func<T, bool>> filter, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            _context.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

       public async Task<bool> UpdateAsync(T entity)
        {
            _context.Update(entity);

            var affected = await _context.SaveChangesAsync();
            return affected > 0; 
            
        }
    }
}

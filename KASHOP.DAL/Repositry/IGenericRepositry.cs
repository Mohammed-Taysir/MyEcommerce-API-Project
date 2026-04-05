using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repositry
{
    public interface IGenericRepositry<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string[]? includes = null);

        Task<T> CreateAsync(T entity);
        Task<T?> GetOne(Expression<Func<T, bool>> filter, string[]? includes = null);
        Task<bool> DeleteAsync(T entity);
        Task<bool> UpdateAsync(T entity);
    }
}

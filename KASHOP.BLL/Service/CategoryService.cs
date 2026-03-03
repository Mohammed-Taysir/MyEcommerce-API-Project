using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepositry _categoryRepositry;
        public CategoryService(ICategoryRepositry categoryRepositry)
        {
            _categoryRepositry = categoryRepositry;
        }
        public async Task<CategoryResponse> CreateCategory(CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            await _categoryRepositry.CreateAsync(category);

            return category.Adapt<CategoryResponse>();
        }

        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            var categories = await _categoryRepositry.GetAllAsync(new string[] { nameof(Category.Translations)});
            return categories.Adapt<List<CategoryResponse>>();
        }

       public async Task<CategoryResponse?> GetCategory(Expression<Func<Category, bool>> filter)
        {
            var category = await _categoryRepositry.GetOne(filter, new string[] { nameof(Category.Translations) });
            return category.Adapt<CategoryResponse>();
        }
    }
}

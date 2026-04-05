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
            var categories = await _categoryRepositry.GetAllAsync(
                p => p.Status == EntityStatus.Active,
                new string[] { nameof(Category.Translations)});

            return categories.Adapt<List<CategoryResponse>>();
        }

        public async Task<CategoryResponse?> GetCategory(Expression<Func<Category, bool>> filter)
        {
            var category = await _categoryRepositry.GetOne(filter, new string[] { nameof(Category.Translations) });
            return category.Adapt<CategoryResponse>();
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryRequest request)
        {
            var category = await _categoryRepositry.GetOne(c => c.Id == id,
                new string[] {nameof(Category.Translations)});
            if (category == null) return false;

            foreach(var translationRequest in request.Translations)
            {
                var existing = category.Translations.FirstOrDefault(t => t.Language == translationRequest.Language);
                if(existing != null)
                {
                    existing.Name = translationRequest.Name;
                }else
                {
                    return false;
                }
            }

            return await _categoryRepositry.UpdateAsync(category);
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _categoryRepositry.GetOne(c => c.Id == id);

            if (category == null) return false;

            return await _categoryRepositry.DeleteAsync(category);
        }


        public async Task<bool> ToggleStatusAsync(int id)
        {
            var category = await _categoryRepositry.GetOne(p => p.Id == id);

            if (category == null) return false;

            category.Status = category.Status == EntityStatus.Active ? EntityStatus.Inactive : EntityStatus.Active;
            return await _categoryRepositry.UpdateAsync(category);
        }


    }
}

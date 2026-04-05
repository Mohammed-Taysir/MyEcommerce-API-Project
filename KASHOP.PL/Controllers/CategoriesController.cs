using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService, IStringLocalizer<SharedResources> localizer)
        {
            _categoryService = categoryService;
            _localizer = localizer;
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            
            var response = await _categoryService.CreateCategory(request);
            return Ok(new
            {
               message = _localizer["Success"].Value,
               response = response
            });
        }

        [HttpGet("")]
     
        public async Task<IActionResult> Index()
        {
            
            var response = await _categoryService.GetAllCategories();
            return Ok(new
            {
                data = response,
                message = _localizer["Success"].Value
            });

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _categoryService.GetCategory(c => c.Id == id));
           
        }

        [HttpPatch("{id}")]
        [Authorize] 
        public async Task<IActionResult> Update(int id, CategoryRequest request)
        {
            var updated = await _categoryService.UpdateCategoryAsync(id, request);

            return updated ? Ok() : BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteCategory(id);
            if (!deleted)
                return NotFound(new {
                message = _localizer["NotFound"].Value
            });

            return Ok(new
            {
                message = _localizer["Success"].Value
            });
        }

        [HttpPatch("Status/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var updated = await _categoryService.ToggleStatusAsync(id);

            if (!updated) return BadRequest();

            return Ok();
        }
    }
}

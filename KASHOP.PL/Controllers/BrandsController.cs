using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public BrandsController(
            IBrandService brandService,
            IStringLocalizer<SharedResources> localizer
            )
        {
            _brandService = brandService;
            _localizer = localizer;
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] BrandRequest request)
        {
            await _brandService.CreateBrandAsync(request);
            return Ok(new
            {
                message = _localizer["Success"].Value
            });
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(new {
                data = brands,
                message = _localizer["Success"].Value
            });

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var brand = await _brandService.GetBrandAsync(id);

            if (brand == null) return BadRequest();

            return Ok(new
            {
                data = brand,
                message = _localizer["Success"].Value
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, BrandUpdateRequest request)
        {
            var updated = await _brandService.UpdateBrandAsync(id, request);
            return updated? Ok(new
            {
                message = _localizer["Success"].Value
            }): BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var deleted = await _brandService.DeleteBrandAsync(id);
            
            return deleted? Ok(new {
                message = _localizer["Success"].Value
            }): BadRequest();
        }
        [HttpPatch("Status/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var updated = await _brandService.ToggleStatusAsync(id);

            if (!updated) return BadRequest();

            return Ok();
        }
    }
}

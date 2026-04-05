using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Models;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IStringLocalizer _localizer;
        public ProductsController(IProductService productService, IStringLocalizer<SharedResources> localizere)
        {
            _productService = productService;
            _localizer = localizere;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            return Ok(new {
                data = products,
                message = "Success"
            });
        }

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductRequest request)
        {
            await _productService.CreateProduct(request);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = _productService.GetProductAsync(p => p.Id == id);
            if (product is null) return BadRequest();

            return Ok(new
            {
                data = product,
                message = "success"
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);

            if (!deleted) return BadRequest();

            return Ok();
        }
        [HttpPatch("{id}")]
        [Authorize]
       public async Task<IActionResult> UpdateProduct(int id, ProductUpdateRequest request)
        {
            var updated = await _productService.UpdateProductAsync(id, request);

            if(!updated) return BadRequest();

            return Ok();
        }

        [HttpPatch("Status/{id}")]
        [Authorize]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var updated = await _productService.ToggleStatusAsync(id);

            if (!updated) return BadRequest();

            return Ok();
        }

    }
}

using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using Mapster;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepositry _productRepositry;
        private readonly IFileService _fileService;

        public ProductService(
            IProductRepositry productRepositry,
            IFileService fileService
            )
        {
            _productRepositry = productRepositry; 
            _fileService = fileService;
        }

        public async Task CreateProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();
            if (request.MainImage != null)
            {
                var imagePath = await _fileService.UploadAsync(request.MainImage);
                product.MainImage = imagePath;
            }
            await _productRepositry.CreateAsync(product);
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync() {
            var products = await _productRepositry.GetAllAsync(
                p => p.Status == EntityStatus.Active,
                new string[]
                {
                    nameof(Product.Translations),
                    nameof(Product.CreatedBy)
                }
                );

            return products.Adapt<List<ProductResponse>>();
        }

        public async Task<ProductResponse?> GetProductAsync(Expression<Func<Product, bool>> filter)
        {
            var product = await _productRepositry.GetOne(filter, new string[]
            {
                nameof(Product.Translations),
                nameof(Product.CreatedBy)
            });

            if (product == null) return null;

            return product.Adapt<ProductResponse>();
        }
        
        public async Task<bool>DeleteProductAsync(int id)
        {
            var product = await _productRepositry.GetOne(p => p.Id == id);

            if (product == null) return false;

            _fileService.Delete(product.MainImage);
            return await _productRepositry.DeleteAsync(product);
        }

        public async Task<bool> UpdateProductAsync(int id, ProductUpdateRequest request)
        {
            var product = await _productRepositry.GetOne(p => p.Id == id,
                new string[] {nameof(Product.Translations)});

            if (product == null) return false;

            request.Adapt(product);

            if(request.Translations != null)
            {
                foreach(var translationRequest in request.Translations)
                {
                    var existing = product.Translations.FirstOrDefault(t => t.Language == translationRequest.Language);
                    if(existing != null)
                    {
                        if(existing.Name != null)
                        {
                            existing.Name = translationRequest.Name;
                        }

                        if (existing.Description != null)
                        {
                            existing.Description = translationRequest.Description;
                        }
                        
                    }else
                    {
                        return false;
                    }
                }
            }

            var oldImage = product.MainImage;
            if(request.MainImage != null)
            {
                _fileService.Delete(oldImage);
                product.MainImage = await _fileService.UploadAsync(request.MainImage);
            } else
            {
                product.MainImage = oldImage;
            }


            return await _productRepositry.UpdateAsync(product);
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var product = await _productRepositry.GetOne(p => p.Id == id);

            if (product == null) return false;

            product.Status = product.Status == EntityStatus.Active ? EntityStatus.Inactive : EntityStatus.Active;
            return await _productRepositry.UpdateAsync(product);
        }     
    }
}

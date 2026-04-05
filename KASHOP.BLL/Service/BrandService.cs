using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepositry _brandRepositry;
        private readonly IFileService _fileService;
        public BrandService(
            IBrandRepositry brandRepositry,
            IFileService fileService
            )
        {
            _brandRepositry = brandRepositry;
            _fileService = fileService;

        }
        public async Task CreateBrandAsync(BrandRequest request)
        {
            var brand = request.Adapt<Brand>();
            if(request.Logo != null)
            {
                brand.Logo = await _fileService.UploadAsync(request.Logo);
            }

            await _brandRepositry.CreateAsync(brand);
        }

        public async Task<List<BrandResponse>> GetAllBrandsAsync()
        {
            var brands = await _brandRepositry.GetAllAsync(
                b => b.Status == EntityStatus.Active,
                new string[]
                {
                    nameof(Brand.Translations)
                }
                );
            return brands.Adapt<List<BrandResponse>>();

        }

        public async Task<BrandResponse?> GetBrandAsync(int id)
        {
            var brand = await _brandRepositry.GetOne(b => b.Id == id,
                new string[]
                {
                    nameof(Brand.Translations)
                });
            if (brand == null) return null;

            return brand.Adapt<BrandResponse>();
        }

        public async Task<bool> UpdateBrandAsync(int id, BrandUpdateRequest request)
        {
            var brand = await _brandRepositry.GetOne(b => b.Id == id,
                new string[] {nameof(Brand.Translations)});
            if (brand == null) return false;

            request.Adapt(brand);
            if (request.Translations != null)
            {
                foreach (var translationRequest in request.Translations)
                {
                    var existing = brand.Translations.FirstOrDefault(t => t.Language == translationRequest.Language);
                    if (existing != null)
                    {
                        if (existing.Name != null)
                        {
                            existing.Name = translationRequest.Name;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            var oldLogo = brand.Logo;

            if(request.Logo != null)
            {
                _fileService.Delete(oldLogo);
                brand.Logo = await _fileService.UploadAsync(request.Logo);
            }else
            {
                brand.Logo = oldLogo;
            }

            return await _brandRepositry.UpdateAsync(brand);
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _brandRepositry.GetOne(b => b.Id == id);
            if (brand == null) return false;

            _fileService.Delete(brand.Logo);
            return await _brandRepositry.DeleteAsync(brand);
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var brand = await _brandRepositry.GetOne(p => p.Id == id);

            if (brand == null) return false;

            brand.Status = brand.Status == EntityStatus.Active ? EntityStatus.Inactive : EntityStatus.Active;
            return await _brandRepositry.UpdateAsync(brand);
        }
    }
}

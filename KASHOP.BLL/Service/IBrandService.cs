using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IBrandService
    {
        Task CreateBrandAsync(BrandRequest request);
        Task<List<BrandResponse>> GetAllBrandsAsync();
        Task<BrandResponse> GetBrandAsync(int id);
        Task<bool> DeleteBrandAsync(int id);
        Task<bool> UpdateBrandAsync(int id, BrandUpdateRequest request);
        Task<bool> ToggleStatusAsync(int id);
    }
}

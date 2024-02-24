using Mango.Service.CartAPI.Models.Dto;

namespace Mango.Service.CartAPI.Services.IServices
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductDTO>> GetProducts();
    }
}

using Mango.Service.OrderAPI.Models.Dto;

namespace Mango.Service.OrderAPI.Services.IServices
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductDTO>> GetProducts();
    }
}

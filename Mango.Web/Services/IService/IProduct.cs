using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface IProduct
    {
        Task<ResponseDTO?> GetAllProductsAsync ( );
        Task<ResponseDTO?> GetProductByIDAsync ( int id );
        Task<ResponseDTO?> GetProductByNameAsync ( string name );
        Task<ResponseDTO?> CreateProducteAsync ( ProductDTO productDTO );
        Task<ResponseDTO?> UpdateProducteAsync ( ProductDTO productDTO );
        Task<ResponseDTO?> DeleteProducteAsync ( int id );
    }
}

using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
    public class ProductService : IProduct
    {
        private readonly IBaseServices _baseServices;

        public ProductService ( IBaseServices baseServices )
        {
            _baseServices = baseServices;
        }
        public async Task<ResponseDTO?> CreateProducteAsync ( ProductDTO productDTO )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = productDTO,
                Url = SD.ProductAPIBase + "/api/product",
                contentType=SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDTO?> DeleteProducteAsync ( int id )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Delete,
                Url = SD.ProductAPIBase + "/api/product/" + id,
            });
        }

        public async Task<ResponseDTO?> GetAllProductsAsync ( )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.ProductAPIBase + "/api/product",

            });
        }

       

        public async Task<ResponseDTO?> GetProductByIDAsync ( int id )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.ProductAPIBase + "/api/product/" + id,
            });
        }

        public async Task<ResponseDTO?> GetProductByNameAsync ( string name )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.ProductAPIBase + "/api/product/" + name,
            });
        }

        public async Task<ResponseDTO?> UpdateProducteAsync ( ProductDTO productDTO )
        {

            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Put,
                Data = ( productDTO ),
                Url = SD.ProductAPIBase + "/api/product",
                contentType=SD.ContentType.MultipartFormData
			});
        }
    }
}

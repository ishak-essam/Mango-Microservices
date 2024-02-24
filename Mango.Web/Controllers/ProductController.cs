using Mango.Web.Models;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProduct _productservice;

        public ProductController(IProduct product)
        {
            _productservice = product;
        }
        public async Task<IActionResult> ProductIndex ( )
        {
            List<ProductDTO>? ProductDTOs = new();

            ResponseDTO responseDTO = await _productservice.GetAllProductsAsync();
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                ProductDTOs = JsonConvert.DeserializeObject<List<ProductDTO>> (Convert.ToString (responseDTO.Result));
            }
            return View (ProductDTOs);
        }

        public async Task<IActionResult> CreateProduct ( )
        {
            return View ();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct( ProductDTO productDTO )
        {
            if ( ModelState.IsValid )
            {
                List<ProductDTO>? ProductsDTOs = new();

                ResponseDTO responseDTO = await _productservice.CreateProducteAsync(productDTO);
                if ( responseDTO != null && responseDTO.IsSuccessful )
                {
                    TempData [ "success" ] = "Product Created Successfully";
                    return RedirectToAction (nameof (ProductIndex));
                }
                else
                {
                    TempData [ "error" ] = responseDTO.Message;
                }
            }
            return View (productDTO);
        }


        public async Task<IActionResult> DeleteProduct ( int ProductId )
        {
            ResponseDTO responseDTO = await _productservice.GetProductByIDAsync(ProductId);
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                ProductDTO   ProductDTOs = JsonConvert.DeserializeObject<ProductDTO> (Convert.ToString (responseDTO.Result));
                return View (ProductDTOs);
            }
            TempData [ "error" ] = responseDTO.Message;

            return NotFound ();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct ( ProductDTO ProductDto )
        {
            ResponseDTO responseDTO = await _productservice.DeleteProducteAsync(ProductDto.ProductId);
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                TempData [ "success" ] = "Product Deleted";
                return RedirectToAction (nameof (ProductIndex));
            }
            else
            {
                TempData [ "error" ] = responseDTO.Message;
            }
            return View (ProductDto);
        }


        public async Task<IActionResult> EditProduct ( int ProductId )
        {
            ResponseDTO? responseDTO = await _productservice.GetProductByIDAsync(ProductId);
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                ProductDTO?   ProductDTOs = JsonConvert.DeserializeObject<ProductDTO> (Convert.ToString (responseDTO.Result));
                return View (ProductDTOs);
            }
                TempData [ "error" ] = responseDTO?.Message;
            return NotFound ();
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct ( ProductDTO ProductDto )
        {
            if ( ModelState.IsValid ) {
                ResponseDTO responseDTO = await _productservice.UpdateProducteAsync(ProductDto);
                if ( responseDTO != null && responseDTO.IsSuccessful )
                {
                    TempData [ "success" ] = "Product Updated ";

                    return RedirectToAction (nameof (ProductIndex));
                }
                else
                {
                    TempData [ "error" ] = responseDTO.Message;
                }
            }
                return View (ProductDto);
        }
    }
}

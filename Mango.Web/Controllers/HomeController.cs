using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IProduct _productservice;
        private readonly ICartService _cartService;

        public HomeController ( ILogger<HomeController> logger , IProduct product,ICartService cartService )
        {
            _logger = logger;
			_productservice = product;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index ( )
		{
			List<ProductDTO>? ProductDTOs = new();

			ResponseDTO? responseDTO = await _productservice.GetAllProductsAsync();
			if ( responseDTO != null && responseDTO.IsSuccessful )
			{
				ProductDTOs = JsonConvert.DeserializeObject<List<ProductDTO>> (Convert.ToString (responseDTO.Result));
			}
			return View (ProductDTOs);
		}

        [Authorize]
        public async Task<IActionResult> Details (int ProductId )
        {
            ProductDTO? ProductDTOs = new();

            ResponseDTO? responseDTO = await _productservice.GetProductByIDAsync(ProductId);
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                ProductDTOs = JsonConvert.DeserializeObject<ProductDTO> (Convert.ToString (responseDTO.Result));
            }
            return View (ProductDTOs);
        }
        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> Details ( ProductDTO productDTO )
        {
            CartDto cartDto =new CartDto(){
                CartHeader=new CartHeaderDTO{
                    UserId=User.Claims.Where(ele=>ele.Type==JwtClaimTypes.Subject )?.FirstOrDefault().Value
            }
            };
            CartDetailsDTO cartDetail=new CartDetailsDTO(){ 
            Count=productDTO.Count,
            ProductId=productDTO.ProductId,

            };

            List<CartDetailsDTO> cartDetailDTOs=new(){ cartDetail };
            cartDto.CartDetails=cartDetailDTOs;
            ResponseDTO? responseDTO = await _cartService.UpsertCartAsyns(cartDto);
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                TempData [ "success" ] = "Item has been added";
                return RedirectToAction (nameof(Index));
            }
            else {
                TempData [ "error" ] = responseDTO.Message.ToString();
            }
            return View (productDTO);
        }


        public IActionResult Privacy ( )
        {
            return View ();
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error ( )
        {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

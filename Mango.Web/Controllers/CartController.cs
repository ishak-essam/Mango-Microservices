using Mango.Web.Models;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private  ICartService _cartService;
        private  IOrderServices _orderServices;

        public CartController(ICartService cartService, IOrderServices orderServices )
        {
            _cartService = cartService;
            _orderServices = orderServices;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex ( )
        {
            return View (await LoadCartDtoBasedOnLoggedInUser());
        }
        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser ( )
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDTO response=await _cartService.GetCartByUserId(userId);
            if (response!=null &&response.IsSuccessful)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto ();
        }

        public async Task<IActionResult> Remove (int CartDetailId )
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO response=await _cartService.RemoveFromCartAsyns(CartDetailId);
            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = "Delete Successfully";
                return RedirectToAction(nameof(CartIndex)); 
            }
            TempData [ "error" ] = "Error Occuered ";
            return View ();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon ( CartDto cartDto)
        {
            ResponseDTO response=await _cartService.ApplyCouponeAsync(cartDto);
            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = "Coupon Added and get discount";
                return RedirectToAction (nameof (CartIndex));
            }
            TempData [ "error" ] = "Error Occuered ";
            return View ();
        }

        public async Task<IActionResult> EmailCart ( CartDto cartDto )
        {
            CartDto cart= await LoadCartDtoBasedOnLoggedInUser();
           cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDTO response=await _cartService.EmailCart(cart);
            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = "Email will be procced and sent shortly";
                return RedirectToAction (nameof (CartIndex));
            }
            TempData [ "error" ] = "Error Occuered ";
            return View ();
        }
        
        public async Task<IActionResult> RemoveCoupon ( CartDto cartDto)
        {
            ResponseDTO response=await _cartService.RemoveCouponeAsync(cartDto);
            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = " Remove Coupon";
                return RedirectToAction (nameof (CartIndex));
            }
            TempData [ "error" ] = "Error Occuered ";
            return View ();
        }

        [Authorize]
        public async Task<IActionResult> CheckOut ( )
        {
            return View (await LoadCartDtoBasedOnLoggedInUser ());
        }
        [HttpPost]
        [ActionName ("CheckOut")]
        public async Task<IActionResult> CheckOut ( CartDto cartDto )
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;
            
            var response = await _cartService.CreateOrder(cart);
            string jsonString = JsonConvert.SerializeObject(cart);
            Console.WriteLine(jsonString);
            OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            if ( response != null  )
            {
                //get stripe session and redirect to stripe to place order
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                StripeRequestDto stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/CheckOut",
                    OrderHeader = orderHeaderDto
                };
                var stripeResponse = await _orderServices.CreateStripeSession(stripeRequestDto);
                StripeRequestDto stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDto>
                                            (Convert.ToString(stripeResponse.Result));
                Response.Headers.Add ("Location", stripeResponseResult.StripeSessionUrl);
                return new StatusCodeResult (303);
            }
            return View ();
        }
        public async Task<IActionResult> Confirmation (int orderId )
        {
            ResponseDTO response=await _orderServices.ValidateStripeSession(orderId);
            if ( response != null && response.IsSuccessful )
            {
                OrderHeaderDto orderHeader=JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                if ( orderHeader.Status == SD.Status_Approved ) {
                    return View (orderId);
                }
            }
                return RedirectToAction (nameof (CartIndex));
        }
    }
}

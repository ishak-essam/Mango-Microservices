using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController ( ICouponService couponService )
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex ( )
        {
            List<CouponDTO>? couponDTOs = new();

            ResponseDTO responseDTO = await _couponService.GetAllCouponsAsync();
            if ( responseDTO != null && responseDTO.IsSuccessful )
            {
                couponDTOs = JsonConvert.DeserializeObject<List<CouponDTO>> (Convert.ToString (responseDTO.Result));
            }
            return View (couponDTOs);
        }

        public async Task<IActionResult> CreateCoupon ( ) {
            return View ();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCoupon ( CouponDTO couponDTO )
        {
            if ( ModelState.IsValid )
            {
                List<CouponDTO>? couponDTOs = new();

                ResponseDTO responseDTO = await _couponService.CreateCouponeAsync(couponDTO);
                if ( responseDTO != null && responseDTO.IsSuccessful )
                {
                    TempData [ "success" ] = " Coupon Created Successfully";

                    return RedirectToAction (nameof (CouponIndex));

                }
                else
                {
                    TempData [ "error" ] = responseDTO.Message;
                }
            }
            return View (couponDTO);
        }
     
        
        
        public async Task<IActionResult> DeleteCoupon ( int couponId )
        {
            ResponseDTO? response = await _couponService.GetCouponByIDAsync(couponId);

            if ( response != null && response.IsSuccessful )
            {
                CouponDTO? model= JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View (model);
            }
            else
            {
                TempData [ "error" ] = response?.Message;
            }
            return NotFound ();
        }
     
        [HttpPost]
        public async Task<IActionResult> DeleteCoupon ( CouponDTO couponDto )
        {
            ResponseDTO? response = await _couponService.DeleteCouponeAsync(couponDto.CouponId);

            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = "Coupon deleted successfully";
                return RedirectToAction (nameof (CouponIndex));
            }
            else
            {
                TempData [ "error" ] = response?.Message;
            }
            return View (couponDto);
        }
       
    }
}

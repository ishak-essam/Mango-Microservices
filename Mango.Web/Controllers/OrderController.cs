using Mango.Web.Models;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderServices _orderServices;

        public OrderController ( IOrderServices orderServices )
        {
            _orderServices = orderServices;
        }
        [Authorize]
        public IActionResult OrderIndex ( )
        {
            return View ();
        }
        [Authorize]
        public async Task<IActionResult> orderDetail ( int orderId)
        {
            OrderHeaderDto orderHeaderDto=new OrderHeaderDto();
            string userid = User.Claims.Where (ele => ele.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault ().Value;
            var responseDTO =await _orderServices.GetOrder(orderId);
            if ( responseDTO != null & responseDTO.IsSuccessful ) 
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto> (Convert.ToString (responseDTO.Result));
            }
            if ( !User.IsInRole (SD.RoleAdmin) && userid != orderHeaderDto.UserId )
            {
                return NotFound ();
            }
                return View (orderHeaderDto);
        }
        [HttpGet]
        public  IActionResult GetAll (string status ) {
            IEnumerable<OrderHeaderDto> list;
            string userid="";
            if ( !User.IsInRole (SD.RoleAdmin) ) {
                userid = User.Claims.Where (ele => ele.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault ().Value;
            }
            ResponseDTO responseDTO =  _orderServices.GetAllOrder(userid).GetAwaiter().GetResult();
            if ( responseDTO != null & responseDTO.IsSuccessful )
            {
                list = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDto>> (Convert.ToString (responseDTO.Result));
                switch ( status )
                {
                    case "approved":
                        list = list.Where (ele => ele.Status == SD.Status_Approved);
                        break;
					case "readyforpickup":
						list = list.Where (ele => ele.Status == SD.Status_ReadyForPickup);
						break;
					case "cancelled":
						list = list.Where (ele => ele.Status == SD.Status_Cancelled);
						break;

				}
            }
            else {
                list = new List<OrderHeaderDto> ();
            }
            Console.WriteLine (list);
            return Json (new { data=list });
        }

        [HttpPost ("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup ( int orderId )
        {
            var response = await _orderServices.UpdateOrderStatus(orderId,SD.Status_ReadyForPickup);
            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = "Status updated successfully";
                return RedirectToAction (nameof (orderDetail), new { orderId = orderId });
            }
            return View ();
        }

        [HttpPost ("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder ( int orderId )
        {
            var response = await _orderServices.UpdateOrderStatus(orderId, SD.Status_Completed);
            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = "Status updated successfully";
                return RedirectToAction (nameof (orderDetail), new { orderId = orderId });
            }
            return View ();
        }

        [HttpPost ("CancelOrder")]
        public async Task<IActionResult> CancelOrder ( int orderId )
        {
            var response = await _orderServices.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if ( response != null && response.IsSuccessful )
            {
                TempData [ "success" ] = "Status updated successfully";
                return RedirectToAction (nameof (orderDetail), new { orderId = orderId });
            }
            return View ();
        }

    }
}

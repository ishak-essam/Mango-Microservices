using Mango.Service.CartAPI.Models.Dto;
using Mango.Services.CartAPI.Models.Dto;

namespace Mango.Service.CartAPI.Services.IServices
{
    public interface ICouponService
    {
         Task<CouponDTO> GetCoupon(string CouponCode);
    }
}

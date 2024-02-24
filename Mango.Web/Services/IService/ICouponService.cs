using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface ICouponService
    {
        Task<ResponseDTO?> GetAllCouponsAsync (   );
        Task<ResponseDTO?> GetCouponAsync ( string CouponCode);
        Task<ResponseDTO?> GetCouponByIDAsync ( int id);
        Task<ResponseDTO?> CreateCouponeAsync ( CouponDTO couponDTO  );
        Task<ResponseDTO?> UpdateCouponeAsync ( CouponDTO couponDTO );
        Task<ResponseDTO?> DeleteCouponeAsync ( int id );

    }
}

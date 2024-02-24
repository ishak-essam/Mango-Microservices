using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;
using System;

namespace Mango.Web.Services
{
    public class CouponServices : ICouponService
    {
        private readonly IBaseServices _baseServices;

        public CouponServices (IBaseServices baseServices )
        {
            _baseServices = baseServices;
        }

        public async Task<ResponseDTO?> CreateCouponeAsync ( CouponDTO couponDTO )
        {
            return await _baseServices.SendAsync (new RequestDTO()
            {
                ApiType = SD.ApiType.Post,
				Data =  couponDTO,
				Url = SD.CouponAPIBase + "/api/coupon",
            });
        }

        public async Task<ResponseDTO?> DeleteCouponeAsync ( int id )
        {
            return await _baseServices.SendAsync (new RequestDTO()
            {
                ApiType = SD.ApiType.Delete,
                Url = SD.CouponAPIBase + "/api/coupon/" + id,
            });
        }

        public async Task<ResponseDTO?> GetAllCouponsAsync ( )
        {
            return await _baseServices.SendAsync (new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url=SD.CouponAPIBase+"/api/coupon",

            }) ;
        }

        public async Task<ResponseDTO?> GetCouponAsync ( string CouponCode )
        {
            return await _baseServices.SendAsync (new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode/" + CouponCode,
            });
        }

        public async Task<ResponseDTO?> GetCouponByIDAsync ( int id )
        {
            return await _baseServices.SendAsync (new RequestDTO()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.CouponAPIBase + "/api/coupon/" + id,
            });
        }

        public async Task<ResponseDTO?> UpdateCouponeAsync ( CouponDTO couponDTO )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Put,
                Data = (couponDTO),
                Url = SD.CouponAPIBase + "/api/coupon",
            });
        }
    }
}

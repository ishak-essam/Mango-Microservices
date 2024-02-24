using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap ( ) {
            var configMap=new MapperConfiguration (config=>{
                config.CreateMap<Coupon,CouponDTO>();
                config.CreateMap<CouponDTO,Coupon>();
            }            );
            return configMap;
        }
    }
}

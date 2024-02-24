using AutoMapper;
using Mango.Service.CartAPI.Models;
using Mango.Service.CartAPI.Models.Dto;

namespace Mango.Services.CartAPI
{
    
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap ( ) {
            var configMap=new MapperConfiguration (config=>{
                config.CreateMap<CartHeader,CartHeaderDTO>().ReverseMap();
                config.CreateMap<CartDetails,CartDetailsDTO>().ReverseMap();;
            }            );
            return configMap;
        }
    }
}

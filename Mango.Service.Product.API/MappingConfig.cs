using AutoMapper;
using Mango.Service.ProductAPI.Models;
using Mango.Service.ProductAPI.Models.Dto;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap ( ) {
            var configMap=new MapperConfiguration (config=>{
                config.CreateMap<Product,ProductDTO>();
                config.CreateMap<ProductDTO,Product>();
            }            );
            return configMap;
        }
    }
}

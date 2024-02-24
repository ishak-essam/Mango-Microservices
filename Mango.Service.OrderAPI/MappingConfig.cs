using AutoMapper;
using Mango.Service.OrderAPI.Models;
using Mango.Service.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI
{
    
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap ( ) {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDto, CartHeaderDTO>()
                .ForMember(dest=>dest.CartTotal, u=>u.MapFrom(src=>src.OrderTotal)).ReverseMap();
                config.CreateMap<CartDetailsDTO, OrderDetailsDTO>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));
                config.CreateMap<OrderDetailsDTO, CartDetailsDTO>();
                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetailsDTO, OrderDetails>().ReverseMap();

            });
            return mappingConfig;
        }
    }
}

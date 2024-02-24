using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;
using System;

namespace Mango.Web.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IBaseServices _baseServices;

        public OrderServices ( IBaseServices baseServices )
        {
            _baseServices = baseServices;
        }


        public async Task<ResponseDTO?> CreateOrder ( CartDto cartDto )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = cartDto,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder",
            });
        }

        public async Task<ResponseDTO?> CreateStripeSession ( StripeRequestDto stripeRequestDto )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = stripeRequestDto,
                Url = SD.OrderAPIBase + "/api/order/CreateStripeSession",
            });
        }

        public async Task<ResponseDTO?> GetAllOrder ( string? userId )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.OrderAPIBase + "/api/order/GetOrders?userId=" + userId
            });
        }

        public async Task<ResponseDTO?> GetOrder ( int userId )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Get,
                Url = SD.OrderAPIBase + "/api/order/GetOrder/" + userId,
            });
        }

        public async Task<ResponseDTO?> UpdateOrderStatus ( int userId, string? status )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = status,
                Url = SD.OrderAPIBase + "/api/order/UpdateOrderStatus/" + userId,
            });
        }

        public async Task<ResponseDTO?> ValidateStripeSession ( int OrderHeaderId )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = OrderHeaderId,
                Url = SD.OrderAPIBase + "/api/order/ValidateStripeSession",
            });
        }
    }
}

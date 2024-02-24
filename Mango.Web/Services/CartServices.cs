using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;

namespace Mango.Web.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseServices _baseServices;

        public CartService ( IBaseServices baseServices )
        {
            _baseServices = baseServices;
        }

        public async Task<ResponseDTO?> ApplyCouponeAsync ( CartDto cartDto )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = cartDto,
                Url = SD.CartAPIBase + "/api/cart/ApplyCoupon",
            });
        }
        public async Task<ResponseDTO?> RemoveCouponeAsync ( CartDto cartDto )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = cartDto,
                Url = SD.CartAPIBase + "/api/cart/RemoveCoupon",
            });
        }
        public async Task<ResponseDTO?> GetCartByUserId ( string UserId )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Url = SD.CartAPIBase + "/api/cart/GetCart/"+ UserId,
            });
        }
        public async Task<ResponseDTO?> RemoveFromCartAsyns ( int CartDetail )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data= CartDetail,
                Url = SD.CartAPIBase + "/api/cart/RemoveCart/",
            });
        }
        public async Task<ResponseDTO?> UpsertCartAsyns ( CartDto cartDto )
        {
        return await _baseServices.SendAsync (new RequestDTO ()
        {
            ApiType = SD.ApiType.Post,
            Data= cartDto,
            Url = SD.CartAPIBase + "/api/cart/UpsertCart" ,
        });
    }
        public async Task<ResponseDTO?> EmailCart ( CartDto cartDto )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = cartDto,
                Url = SD.CartAPIBase + "/api/cart/EmailCartRequest",
            });
        }
        public async Task<ResponseDTO?> CreateOrder ( CartDto cartDto )
        {
            string jsonString = JsonConvert.SerializeObject(cartDto);
            Console.WriteLine (jsonString);
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = cartDto,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder"
            });
        }
    }
}

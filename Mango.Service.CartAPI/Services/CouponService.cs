using Mango.Service.CartAPI.Services.IServices;
using Mango.Services.CartAPI.Models.Dto;
using Newtonsoft.Json;

namespace Mango.Service.CartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDTO> GetCoupon ( string CouponCode )
        {
            var client=_httpClientFactory.CreateClient("Coupon");
            var response=await client.GetAsync($"/api/coupon/GetByCode/"+CouponCode);
            var apiConent=await response.Content.ReadAsStringAsync();
            var resp=JsonConvert.DeserializeObject<ResponseDTO>(apiConent);
            if ( resp.IsSuccessful )
            {
                return JsonConvert.DeserializeObject<CouponDTO> (Convert.ToString (resp.Result));
            }
            return new CouponDTO();
        }
    }
}

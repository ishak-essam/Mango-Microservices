﻿using Mango.Service.OrderAPI.Models.Dto;
using Mango.Service.OrderAPI.Services.IServices;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mango.Service.OrderAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDTO>> GetProducts ( )
        {
            var client=_httpClientFactory.CreateClient("Product");
            var response=await client.GetAsync($"/api/Product");
            var apiConent=await response.Content.ReadAsStringAsync();
            var resp=JsonConvert.DeserializeObject<ResponseDTO>(apiConent);
            if ( resp.IsSuccessful ) {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>> (Convert.ToString(resp.Result));
            }
            return new List<ProductDTO>();
        }
    }
}

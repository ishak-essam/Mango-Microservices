using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Service.OrderAPI.Models.Dto
{
    public class OrderDetailsDTO
    {
        public int OrderDetailId { get; set; }
        public int cartHeadersId { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}

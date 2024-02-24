using Mango.Service.OrderAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Service.OrderAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int cartHeadersId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader orderHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}

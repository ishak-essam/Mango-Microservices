

namespace  Mango.Web.Models
{
    public class CartDetailsDTO
    {
        public int CartDetailId { get; set; }
        public int cartHeadersId { get; set; }
        public CartHeaderDTO? cartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }
    }
}

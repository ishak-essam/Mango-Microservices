namespace Mango.Web.Models
{
    public class CartDto
    {
        public CartHeaderDTO CartHeader { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
    }
}

namespace Mango.Service.OrderAPI.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDTO CartHeader { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
    }
}

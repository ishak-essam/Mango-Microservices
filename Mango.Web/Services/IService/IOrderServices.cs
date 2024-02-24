using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface IOrderServices
    {
        Task<ResponseDTO?> CreateOrder (  CartDto cartDto );
        Task<ResponseDTO?> CreateStripeSession (  StripeRequestDto stripeRequestDto );
        Task<ResponseDTO?> ValidateStripeSession (  int OrderHeaderId );
        Task<ResponseDTO?> GetAllOrder (  string? userId );
        Task<ResponseDTO?> GetOrder (  int userId );
        Task<ResponseDTO?> UpdateOrderStatus (  int userId, string? status );
        
    }
}

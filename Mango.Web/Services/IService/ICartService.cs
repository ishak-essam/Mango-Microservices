using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface ICartService
    {
        Task<ResponseDTO?> GetCartByUserId ( string UserId );
        Task<ResponseDTO?> UpsertCartAsyns ( CartDto cartDto);
        Task<ResponseDTO?> RemoveFromCartAsyns ( int CartDetail);
        Task<ResponseDTO?> ApplyCouponeAsync ( CartDto cartDto );
        Task<ResponseDTO?> RemoveCouponeAsync ( CartDto cartDto );
        Task<ResponseDTO?> EmailCart ( CartDto cartDto );
        Task<ResponseDTO?> CreateOrder ( CartDto cartDto );
    
    }
}

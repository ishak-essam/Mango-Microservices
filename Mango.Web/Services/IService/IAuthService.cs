using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
        Task<ResponseDTO?> AssignAsync( RegistrationRequestDTO registrationRequestDTO );
    }
}

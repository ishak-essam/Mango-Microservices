using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseServices _baseServices;

        public AuthService ( IBaseServices baseServices )
        {
            _baseServices = baseServices;
        }

        public  async Task<ResponseDTO?> AssignAsync ( RegistrationRequestDTO registrationRequestDTO )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = registrationRequestDTO,
                Url = SD.AuthAPIBase + "/api/auth/assignRole",
            });
        }

        public async  Task<ResponseDTO?> LoginAsync ( LoginRequestDTO loginRequestDTO )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = loginRequestDTO,
                Url = SD.AuthAPIBase + "/api/auth/login",
            },withBearer:false);
        }

        public async Task<ResponseDTO?> RegisterAsync ( RegistrationRequestDTO registrationRequestDTO )
        {
            return await _baseServices.SendAsync (new RequestDTO ()
            {
                ApiType = SD.ApiType.Post,
                Data = registrationRequestDTO,
                Url = SD.AuthAPIBase + "/api/auth/register",
            });
        }
    }
}

using Mango.Service.AuthAPI.Models.Dto;

namespace Mango.Service.AuthAPI.Service.IService
{
    public interface IAuth
    {
        Task<string> Register(RegistrationRequestDTO requestDTO);   
        Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO); 
        Task<bool> AssignRole(string  email,string roleName);
    }
}

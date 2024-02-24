using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface IBaseServices
    {
       Task<ResponseDTO?> SendAsync ( RequestDTO requestDTO ,bool withBearer=true);
    }
}

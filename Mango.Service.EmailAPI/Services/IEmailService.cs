using Mango.Service.EmailAPI.Message;
using Mango.Service.EmailAPI.Models.Dto;

namespace Mango.Service.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog ( CartDto cartDto );
        Task RegisterUserEmailAndLog ( string email );
        Task LogOrderPlaced ( RewardMessage rewardMessage );
    }
}
